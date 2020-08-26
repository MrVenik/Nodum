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
        private object _object;

        private MethodInfo _methodInfo;

        public MethodNode(MethodInfo method, object obj) : base(method.Name)
        {
            if (!method.IsStatic)
            {
                if (method.IsGenericMethod)
                {
                    throw new Exception("MethodNode can't hold generic method, use GenericMethodNode");
                }

                _object = obj;
                _methodInfo = method;

                try
                {
                    ParameterInfo[] parameters = _methodInfo.GetParameters();

                    NodePin[] parameterPins = NodePinBuilder.BuildNodePins(parameters, this);

                    foreach (var parameterPin in parameterPins)
                    {
                        ProtectedTryAddNodePin(parameterPin);
                    }

                    if (_methodInfo.ReturnType != typeof(void))
                    {
                        NodePin returnNodePin = NodePinBuilder.BuildNodePin("Return", this, _methodInfo.ReturnType, new NodePinOptions() { IsOutput = true });
                        ProtectedTryAddNodePin(returnNodePin);
                    }
                }
                catch (Exception ex)
                {
                    throw new NodeException($"Cant build MethodNode with method {method}. Exception: {ex.Message}");
                }
            }
            else throw new Exception("MethodNode can't hold static method, use StaticMethodNode");
        }

        public override void UpdateValue()
        {
            if (_methodInfo != null && _object != null)
            {
                List<object> parameterValues = new List<object>();

                ParameterInfo[] parameters = _methodInfo.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (NodePins.TryGetValue(parameter.Name, out NodePin nodePin))
                    {
                        parameterValues.Add(nodePin.Value);
                    }
                }

                if (NodePins.ContainsKey("Return"))
                {
                    NodePins["Return"].Value = _methodInfo.Invoke(_object, parameterValues.ToArray());
                }
                else
                {
                    _methodInfo.Invoke(null, parameterValues.ToArray());
                }
            }
        }
    }
}
