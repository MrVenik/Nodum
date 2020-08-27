using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Nodum.Reflection
{
    public class GenericStaticMethodNode : Node
    {
        private List<NodePin> _genericArgumentPins = new List<NodePin>();

        private bool _isParametersGeneric = false;
        private bool _isReturnGeneric = false;

        private MethodInfo _methodInfo;
        public GenericStaticMethodNode(MethodInfo method) : base(method.Name)
        {
            if (method.IsStatic)
            {
                if (!method.IsGenericMethod)
                {
                    throw new Exception("GenericStaticMethodNode can't hold not generic method, use StaticMethodNode");
                }

                _methodInfo = method;
                try
                {
                    Type[] genericArguments = method.GetGenericArguments();

                    _isParametersGeneric = genericArguments.Length > 0;
                    _isReturnGeneric = _methodInfo.ReturnType.ContainsGenericParameters;

                    foreach (var arg in genericArguments)
                    {
                        NodePin genericArgumentPin = NodePinBuilder.BuildNodePin(arg.Name, this, typeof(Type), new NodePinOptions() { IsInvokeUpdatePins = true });
                        _genericArgumentPins.Add(genericArgumentPin);
                        ProtectedTryAddNodePin(genericArgumentPin);
                    }


                    ParameterInfo[] parameters = _methodInfo.GetParameters();

                    foreach (var parameter in parameters)
                    {
                        if (!parameter.ParameterType.ContainsGenericParameters)
                        {
                            ProtectedTryAddNodePin(NodePinBuilder.BuildNodePin(parameter, this));
                        }
                    }

                    if (_methodInfo.ReturnType != typeof(void))
                    {
                        if (!_methodInfo.ReturnType.ContainsGenericParameters)
                        {
                            ProtectedTryAddNodePin(NodePinBuilder.BuildNodePin("Return", this, _methodInfo.ReturnType, new NodePinOptions() { IsOutput = true }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new NodeException($"Cant build StaticMethodNode with method {method}. Exception: {ex.Message}");
                }
            }
            else throw new NodeException("GenericStaticMethodNode can't hold instance method, use GenericMethodNode");
        }

        public override void UpdatePins()
        {
            (string, Type)[] genericArguments = new (string, Type)[_genericArgumentPins.Count];

            for (int i = 0; i < _genericArgumentPins.Count; i++)
            {
                NodePin arg = _genericArgumentPins[i];
                if (arg.Value != null)
                {
                    genericArguments[i] = (arg.Name, (Type)arg.Value);
                }
                else return;
            }

            if (_isParametersGeneric)
            {
                ParameterInfo[] parameters = _methodInfo.GetParameters();

                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterType.ContainsGenericParameters)
                    {
                        ProtectedTryAddNodePin(NodePinBuilder.BuildGenericNodePin(parameter.Name, this, parameter.ParameterType, new NodePinOptions() { IsInput = true, IsInvokeUpdate = true }, genericArguments));
                    }
                }
            }

            if (_isReturnGeneric)
            {
                ProtectedTryAddNodePin(NodePinBuilder.BuildGenericNodePin("Return", this, _methodInfo.ReturnType, new NodePinOptions() { IsOutput = true }, genericArguments));
            }
        }

        public override void UpdateValue()
        {
            if (_methodInfo != null)
            {
                List<object> parameterValues = new List<object>();

                ParameterInfo[] parameters = _methodInfo.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (NodePins.TryGetValue(parameter.Name, out NodePin nodePin))
                    {
                        parameterValues.Add(nodePin.Value);
                    }
                    else return;
                }

                if (NodePins.ContainsKey("Return"))
                {
                    NodePins["Return"].Value = _methodInfo.Invoke(null, parameterValues.ToArray());
                }
                else
                {
                    _methodInfo.Invoke(null, parameterValues.ToArray());
                }
            }
        }
    }
}
