using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace Rocket.Sandbox
{
    //todo: use mono cecil instead
    public class Disassembler
    {
        private BinaryReader _stream;
        private OpCode[] _singleByteOpCode;
        private OpCode[] _doubleByteOpCode;
        private byte[] _instructions;
        private IList<LocalVariableInfo> _locals;
        private ParameterInfo[] _parameters;
        private Type[] _typeArgs;
        private Type[] _methodArgs;
        private MethodBase _currentMethod;
        private List<Instruction> _ilInstructions;

        public Disassembler()
        {
            CreateOpCodes();
        }

        private void CreateOpCodes()
        {
            _singleByteOpCode = new OpCode[225];
            _doubleByteOpCode = new OpCode[31];

            FieldInfo[] fields = GetOpCodeFields();

            foreach (FieldInfo field in fields)
            {
                OpCode code = (OpCode)field.GetValue(null);

                if (code.OpCodeType == OpCodeType.Nternal)
                    continue;

                if (code.Size == 1)
                    _singleByteOpCode[code.Value] = code;
                else
                    _doubleByteOpCode[code.Value & 0xff] = code;
            }
        }

        public List<Instruction> ReadInstructions(MethodBase method)
        {
            _ilInstructions = new List<Instruction>();
            this._currentMethod = method;

            var body = method.GetMethodBody();
            _parameters = method.GetParameters();
            if (body == null)
                return _ilInstructions;
            _locals = body.LocalVariables;
            _instructions = method.GetMethodBody().GetILAsByteArray();
            var str = new MemoryStream(_instructions); //Todo
            _stream = new BinaryReader(str);

            if (!(method is ConstructorInfo))
                _methodArgs = method.GetGenericArguments();

            if (method.DeclaringType != null)
                _typeArgs = method.DeclaringType.GetGenericArguments();

            while (_stream.BaseStream.Position < _stream.BaseStream.Length)
            {
                var instruction = new Instruction();
                bool isDoubleByte = false;
                OpCode code = ReadOpCode(ref isDoubleByte);
                instruction.OpCode = code;
                instruction.Offset = _stream.BaseStream.Position - 1;
                if (isDoubleByte)
                {
                    instruction.Offset--;
                }
                instruction.Operand = GetOperand(code, method.Module, ref instruction.LocalVariableIndex);
                _ilInstructions.Add(instruction);
            }

            return _ilInstructions;
        }

        private object GetOperand(OpCode code, Module module, ref long localVariableIndex)
        {
            object operand = null;

            switch (code.OperandType)
            {
                case OperandType.InlineNone:
                    break;
                case OperandType.InlineSwitch:
                    int length = _stream.ReadInt32();
                    int[] branches = new int[length];
                    int[] offsets = new int[length];
                    for (int i = 0; i < length; i++)
                    {
                        offsets[i] = _stream.ReadInt32();
                    }
                    for (int i = 0; i < length; i++)
                    {
                        branches[i] = (int)_stream.BaseStream.Position + offsets[i];
                    }

                    operand = (object)branches; // Just forget to save readed offsets
                    break;
                case OperandType.ShortInlineBrTarget:
                    if (code.FlowControl != FlowControl.Branch && code.FlowControl != FlowControl.Cond_Branch)
                    {
                        operand = _stream.ReadSByte();
                    }
                    else
                    {
                        operand = _stream.ReadSByte() + _stream.BaseStream.Position;
                    }
                    break;
                case OperandType.InlineBrTarget:
                    operand = _stream.ReadInt32() + _stream.BaseStream.Position;
                    break;
                case OperandType.ShortInlineI:
                    if (code == OpCodes.Ldc_I4_S)
                        operand = (sbyte)_stream.ReadByte();
                    else
                        operand = _stream.ReadByte();
                    break;
                case OperandType.InlineI:
                    operand = _stream.ReadInt32();
                    break;
                case OperandType.ShortInlineR:
                    operand = _stream.ReadSingle();
                    break;
                case OperandType.InlineR:
                    operand = _stream.ReadDouble();
                    break;
                case OperandType.InlineI8:
                    operand = _stream.ReadInt64();
                    break;
                case OperandType.InlineSig:
                    operand = module.ResolveSignature(_stream.ReadInt32());
                    break;
                case OperandType.InlineString:
                    operand = module.ResolveString(_stream.ReadInt32());
                    break;
                case OperandType.InlineTok:
                case OperandType.InlineType:
                case OperandType.InlineMethod:
                case OperandType.InlineField:
                    operand = module.ResolveMember(_stream.ReadInt32(), _typeArgs, _methodArgs);
                    break;
                case OperandType.ShortInlineVar:
                    {
                        int index = _stream.ReadByte();
                        operand = GetVariable(code, index);
                        localVariableIndex = index;
                    }
                    break;
                case OperandType.InlineVar:
                    {
                        int index = _stream.ReadUInt16();
                        operand = GetVariable(code, index);
                        localVariableIndex = index;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            return operand;
        }

        private OpCode ReadOpCode(ref bool isDoubleByte)
        {
            isDoubleByte = false;
            byte instruction = _stream.ReadByte();
            if (instruction != 254)
                return _singleByteOpCode[instruction];
            else
            {
                isDoubleByte = true;
                return _doubleByteOpCode[_stream.ReadByte()];
            }
        }

        private object GetVariable(OpCode code, int index)
        {
            if (code.Name.Contains("loc"))
                return _locals[index];

            if (!_currentMethod.IsStatic)
                index--;

            return _parameters[index];
        }

        private FieldInfo[] GetOpCodeFields()
        {
            return typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
        }

        public IList<LocalVariableInfo> Locals => _locals;
    }
}