using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Rocket.SandboxTester
{
    public class CheckResult
    {
        public ReadableInstruction IllegalInstruction { get; set; }
        public ReadableInstruction Position { get; set; }
        public bool Passed => IllegalInstruction == null;
    }

    public class ReadableInstruction
    {
        public string InstructionName { get; private set; }
        public object InstructionObject { get; }
        public InstructionType InstructionType { get; }
        public BlockReason BlockReason { get; }

        public ReadableInstruction(MethodBase method, BlockReason reason = BlockReason.RESTRICTED)
        {
            InstructionName = (method.DeclaringType == null ? "" : method.DeclaringType.FullName + ".") + method.Name;
            InstructionObject = method;
            InstructionType = InstructionType.METHOD;
            BlockReason = reason;
            AppendInstructionType();
        }

        public ReadableInstruction(Type type, BlockReason reason = BlockReason.RESTRICTED)
        {
            InstructionName = string.IsNullOrEmpty(type.FullName) ? type.Name : type.FullName;
            InstructionObject = type;
            InstructionType = InstructionType.TYPE;
            BlockReason = reason;
        }

        public ReadableInstruction(MethodAttributes attributes, BlockReason reason = BlockReason.RESTRICTED)
        {
            var flags = Enum.GetValues(typeof(MethodAttributes)).Cast<MethodAttributes>()
                .Where(f => (attributes & f) == 0)
                .ToList();

            InstructionName = "[" + string.Join(", ", flags) + "]";
            InstructionObject = flags;
            InstructionType = InstructionType.METHOD_ATTRIBUTE;
            BlockReason = reason;
            AppendInstructionType();
        }

        public ReadableInstruction(MethodBase method, Instruction ins, BlockReason reason = BlockReason.RESTRICTED)
        {
            InstructionName = (method.DeclaringType == null ? "" : method.DeclaringType.FullName + ".") + method.Name + " (on operand: " + ins.OpCode + " @@ 0x" + ins.Offset.ToString("X") + ")";
            InstructionObject = new MethodInstruction(method, ins);
            InstructionType = InstructionType.OPERAND;
            BlockReason = reason;
            AppendInstructionType();
        }

        public override bool Equals(object o)
        {
            if (!(o is ReadableInstruction))
                return false;

            return Equals((ReadableInstruction) o);
        }

        protected bool Equals(ReadableInstruction other)
        {
            return Equals(InstructionObject, other.InstructionObject);
        }

        public override int GetHashCode()
        {
            return InstructionObject?.GetHashCode() ?? 0;
        }

        private void AppendInstructionType()
        {
            InstructionName = "[" + InstructionType + "] " + InstructionName;
        }

        public override string ToString()
        {
            return InstructionName;
        }
    }

    public class MethodInstruction
    {
        public MethodBase Method { get; }
        public Instruction Instruction { get; }

        public MethodInstruction(MethodBase method, Instruction ins)
        {
            Method = method;
            Instruction = ins;
        }
    }

    public enum BlockReason
    {
        RESTRICTED
    }

    public enum InstructionType
    {
        TYPE,
        METHOD,
        OPERAND,
        METHOD_ATTRIBUTE
    }
}