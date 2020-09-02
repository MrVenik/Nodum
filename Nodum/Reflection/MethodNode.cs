using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
                        SetMethodInstance(obj);
                    }
                    if (IsGeneric)
                    {
                        CreateGenericArgumentPins();
                    }

                    CreateParameterPins();

                    CreateReturnPin();
                }
                catch (Exception ex)
                {
                    throw new NodeException($"Cant build MethodNode with method {method}. Exception: {ex.Message}");
                }
            }
            else throw new NodeException("Cant build MethodNode because MethodInfo is null");
        }

        private void CreateReturnPin()
        {
            if (_methodInfo.ReturnType != typeof(void) && !_methodInfo.ReturnType.ContainsGenericParameters)
            {
                NodePinOptions options = new NodePinOptions() { IsOutput = true };
                NodePin returnNodePin = NodePinBuilder.BuildNodePin("Return", this, _methodInfo.ReturnType, options);
                ProtectedTryAddNodePin(returnNodePin);
            }
        }

        private void CreateParameterPins()
        {
            ParameterInfo[] parameters = _methodInfo.GetParameters();

            foreach (var parameter in parameters)
            {
                if (!parameter.ParameterType.ContainsGenericParameters)
                {
                    ProtectedTryAddNodePin(NodePinBuilder.BuildNodePin(parameter, this));
                }
            }
        }

        private void CreateGenericArgumentPins()
        {
            _genericArgumentPins = new List<NodePin>();

            Type[] genericArguments = _methodInfo.GetGenericArguments();

            foreach (var arg in genericArguments)
            {
                NodePinOptions options = new NodePinOptions() { IsInput = true, IsInvokeUpdatePins = true };
                NodePin genericArgumentPin = NodePinBuilder.BuildNodePin(arg.Name, this, typeof(Type), options);
                _genericArgumentPins.Add(genericArgumentPin);
                ProtectedTryAddNodePin(genericArgumentPin);
            }
        }

        private void SetMethodInstance(object obj)
        {
            if (obj == null)
            {
                NodePinOptions options = new NodePinOptions() { IsInput = true, IsInvokeUpdate = true };
                NodePin objectNodePin = NodePinBuilder.BuildNodePin("Object", this, _methodInfo.ReflectedType, options);
                ProtectedTryAddNodePin(objectNodePin);
            }
            else
            {
                _object = obj;
            }
        }

        public override void UpdatePins()
        {
            if (IsGeneric)
            {
                (string, Type)[] genericArguments = new (string, Type)[_genericArgumentPins.Count];

                bool isGenericArgumentsSetted = TrySetGenericArguments(genericArguments);

                if (isGenericArgumentsSetted)
                {
                    CreateGenericParameterPins(genericArguments);

                    CreateGenericReturnPin(genericArguments);
                }
            }
        }

        private void CreateGenericReturnPin((string, Type)[] genericArguments)
        {
            if (_methodInfo.ReturnType != typeof(void) && _methodInfo.ReturnType.ContainsGenericParameters)
            {
                NodePinOptions options = new NodePinOptions() { IsOutput = true };
                NodePin returnNodePin = NodePinBuilder.BuildGenericNodePin("Return", this, _methodInfo.ReturnType, options, genericArguments);
                ProtectedTryAddNodePin(returnNodePin);
            }
        }

        private void CreateGenericParameterPins((string, Type)[] genericArguments)
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
        }

        private bool TrySetGenericArguments((string, Type)[] genericArguments)
        {
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

            return isGenericArgumentsSetted;
        }

        public override void UpdateValue()
        {
            if (_methodInfo != null)
            {
                List<object> parameterValues = new List<object>();

                bool isCanInvoke = TrySetParameterValues(parameterValues);

                if (IsInstance && _object == null)
                {
                    isCanInvoke = TrySetObjectValue();
                }

                if (isCanInvoke)
                {
                    InvokeMethod(parameterValues.ToArray());
                }
            }
        }

        private void InvokeMethod(object[] parameterValues)
        {
            if (_methodInfo.ReturnType != typeof(void))
            {
                if (NodePins.TryGetValue("Return", out NodePin returnNodePin))
                {
                    returnNodePin.Value = _methodInfo.Invoke(_object, parameterValues);
                }
            }
            else
            {
                _methodInfo.Invoke(_object, parameterValues);
            }
        }

        private bool TrySetObjectValue()
        {
            bool isCanInvoke = true;
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

            return isCanInvoke;
        }

        private bool TrySetParameterValues(List<object> parameterValues)
        {
            bool isCanInvoke = true;
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

            return isCanInvoke;
        }

        public override string GetStringForNodePin(NodePin nodePin)
        {
            if (nodePin.Node == this)
            {
                if (nodePin.Name == "Return")
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    ParameterInfo[] parameters = _methodInfo.GetParameters();

                    if (NodePins.ContainsKey("Object"))
                    {
                        stringBuilder.Append($"{GetStringForNodePin(NodePins["Object"])}.");
                    }

                    stringBuilder.Append($"{_methodInfo.Name}(");

                    foreach (var parameter in parameters)
                    {
                        stringBuilder.Append($"{GetStringForNodePin(NodePins[parameter.Name])}, ");
                    }
                    stringBuilder.Append(")");

                    return stringBuilder.ToString();
                }
            }
            return base.GetStringForNodePin(nodePin);
        }
    }
}
