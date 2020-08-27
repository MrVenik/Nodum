using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nodum.Reflection
{
    [Serializable]
    [Node(Group = "Reflection", NodeCacherIgnore = true)]
    public class MethodNode : Node
    {
        public bool IsInstance => !_methodInfo.IsStatic;
        public bool IsStatic => _methodInfo.IsStatic;
        public bool IsGeneric => _methodInfo.IsGenericMethod;

        private object _object;
        private MethodInfo _methodInfo;
        private List<NodePin> _genericArgumentPins;

        public MethodNode(MethodInfo method, object obj = null) : base(method.Name)
        {
            if (method != null)
            {
                try
                {
                    _methodInfo = method;
                    if (IsInstance)
                    {
                        if (obj == null)
                        {
                            NodePinOptions options = new NodePinOptions() { IsInput = true, IsInvokeUpdate = true };
                            NodePin objectNodePin = NodePinBuilder.BuildNodePin("Object", this, typeof(object), options);
                            ProtectedTryAddNodePin(objectNodePin);
                        }
                        else
                        {
                            _object = obj;
                        }
                    }
                    if (IsGeneric)
                    {
                        _genericArgumentPins = new List<NodePin>();

                        Type[] genericArguments = method.GetGenericArguments();

                        foreach (var arg in genericArguments)
                        {
                            NodePinOptions options = new NodePinOptions() { IsInput = true, IsInvokeUpdatePins = true };
                            NodePin genericArgumentPin = NodePinBuilder.BuildNodePin(arg.Name, this, typeof(Type), options);
                            _genericArgumentPins.Add(genericArgumentPin);
                            ProtectedTryAddNodePin(genericArgumentPin);
                        }
                    }

                    ParameterInfo[] parameters = _methodInfo.GetParameters();

                    foreach (var parameter in parameters)
                    {
                        if (!parameter.ParameterType.ContainsGenericParameters)
                        {
                            ProtectedTryAddNodePin(NodePinBuilder.BuildNodePin(parameter, this));
                        }
                    }

                    if (_methodInfo.ReturnType != typeof(void) && !_methodInfo.ReturnType.ContainsGenericParameters)
                    {
                        NodePinOptions options = new NodePinOptions() { IsOutput = true };
                        NodePin returnNodePin = NodePinBuilder.BuildNodePin("Return", this, _methodInfo.ReturnType, options);
                        ProtectedTryAddNodePin(returnNodePin);
                    }
                }
                catch (Exception ex)
                {
                    throw new NodeException($"Cant build MethodNode with method {method}. Exception: {ex.Message}");
                }
            }
            else throw new NodeException("Cant build MethodNode because MethodInfo is null");
        }

        public override void UpdatePins()
        {
            if (IsGeneric)
            {
                (string, Type)[] genericArguments = new (string, Type)[_genericArgumentPins.Count];

                bool isGenericArgumentsSetted = true;

                for (int i = 0; i < _genericArgumentPins.Count; i++)
                {
                    NodePin arg = _genericArgumentPins[i];
                    if (arg.Value != null)
                    {
                        genericArguments[i] = (arg.Name, (Type)arg.Value);
                    }
                    else
                    {
                        isGenericArgumentsSetted = false;
                        break;
                    }
                }

                if (isGenericArgumentsSetted)
                {
                    ParameterInfo[] parameters = _methodInfo.GetParameters();

                    foreach (var parameter in parameters)
                    {
                        if (parameter.ParameterType.ContainsGenericParameters)
                        {
                            NodePinOptions options = new NodePinOptions() { IsInput = true, IsInvokeUpdate = true };
                            NodePin genericParameterNodePin = NodePinBuilder.BuildGenericNodePin(parameter.Name, this, parameter.ParameterType, options, genericArguments);
                            ProtectedTryAddNodePin(genericParameterNodePin);
                        }
                    }

                    if (_methodInfo.ReturnType != typeof(void) && _methodInfo.ReturnType.ContainsGenericParameters)
                    {
                        NodePinOptions options = new NodePinOptions() { IsOutput = true };
                        NodePin returnNodePin = NodePinBuilder.BuildGenericNodePin("Return", this, _methodInfo.ReturnType, options, genericArguments);
                        ProtectedTryAddNodePin(returnNodePin);
                    }
                }
            }
        }

        public override void UpdateValue()
        {
            if (_methodInfo != null)
            {
                bool isCanInvoke = true;

                List<object> parameterValues = new List<object>();

                ParameterInfo[] parameters = _methodInfo.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (NodePins.TryGetValue(parameter.Name, out NodePin nodePin))
                    {
                        parameterValues.Add(nodePin.Value);
                    }
                    else
                    {
                        isCanInvoke = false;
                        break;
                    }
                }


                if (IsInstance && _object == null)
                {
                    if (NodePins.TryGetValue("Object", out NodePin objectNodePin))
                    {
                        _object = objectNodePin.Value;
                        if (_object == null)
                        {
                            isCanInvoke = false;
                        }
                    }
                    else
                    {
                        isCanInvoke = false;
                    }
                }

                if (isCanInvoke)
                {
                    if (_methodInfo.ReturnType != typeof(void))
                    {
                        if (NodePins.TryGetValue("Return", out NodePin returnNodePin))
                        {
                            returnNodePin.Value = _methodInfo.Invoke(_object, parameterValues.ToArray());
                        }
                    }
                    else
                    {
                        _methodInfo.Invoke(_object, parameterValues.ToArray());
                    }
                }
            }
        }
    }
}
