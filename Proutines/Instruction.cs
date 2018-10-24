using System.Collections.Generic;

namespace Proutines
{
    public static class Instruction
    {
        public static IYieldInstruction All(IYieldInstruction[] instructions)
        {
            return All(instructions as IEnumerable<IYieldInstruction>);
        }

        public static IYieldInstruction All(IEnumerable<IYieldInstruction> instructions)
        {
            IYieldInstruction finalInstruction = null;

            foreach (var instruction in instructions)
            {
                finalInstruction = finalInstruction == null ? instruction : finalInstruction.And(instruction);
            }

            return finalInstruction;
        }

        public static IYieldOperation<int> First(IYieldInstruction[] instructions)
        {
            return First(instructions as IEnumerable<IYieldInstruction>);
        }

        public static IYieldOperation<int> First(IEnumerable<IYieldInstruction> instructions)
        {
            IYieldOperation<int> finalInstruction = null;
            IYieldInstruction first = null;
            var count = 0;

            foreach (var instruction in instructions)
            {
                count++;

                switch (count)
                {
                    case 1:
                        first = instruction;
                        break;
                    case 2:
                        finalInstruction = new OrInt(first, instruction, 0, 1);
                        break;
                    default:
                        finalInstruction = new OrInt2(finalInstruction, instruction, count - 1);
                        break;
                }
            }

            return count == 1 ? new OrInt(first, first, 0, 0) : finalInstruction;
        }
    }
}