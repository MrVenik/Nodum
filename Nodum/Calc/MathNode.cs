﻿using Nodum.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nodum.Calc
{
    [Serializable]
    public class MathNode : Node
    {
        [Serializable]
        public enum MathType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Remainder,
            Pow,
            Root,
        }

        public override bool IsBaseNode { get; }

        public MathNode(string name = "Math Node") : base(name)
        {
            IsBaseNode = true;
        }

        [NodePin(IsInvokeUpdate = true, CanSetValue = true)] public MathType Operation;
        [Input] public double InputA { get; set; }
        [Input] public double InputB { get; set; }
        [Output] public double Result { get; set; }

        public override void UpdateValue()
        {
            Result = 0;
            switch (Operation)
            {
                case MathType.Add:
                    Result = InputA + InputB;
                    break;
                case MathType.Subtract:
                    Result = InputA - InputB;
                    break;
                case MathType.Multiply:
                    Result = InputA * InputB;
                    break;
                case MathType.Divide:
                    if (InputB != 0)
                    {
                        Result = InputA / InputB;
                    }
                    break;
                case MathType.Remainder:
                    if (InputB != 0)
                    {
                        Result = InputA % InputB;
                    }
                    break;
                case MathType.Pow:
                    Result = Math.Pow(InputA, InputB);
                    break;
                case MathType.Root:
                    Result = Math.Pow(InputA, 1.0 / InputB);
                    break;
            }
        }
    }
}