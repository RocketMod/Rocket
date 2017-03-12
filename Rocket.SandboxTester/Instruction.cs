using System.Reflection.Emit;

namespace Rocket.SandboxTester
{
    public class Instruction
    {
        public OpCode OpCode;
        public object Operand;
        public long Offset;
        public long LocalVariableIndex;
    }
}