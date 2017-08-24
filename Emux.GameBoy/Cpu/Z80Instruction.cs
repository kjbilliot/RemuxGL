﻿using System;

namespace Emux.GameBoy.Cpu
{
    /// <summary>
    /// Represents an instruction in the Z80 instruction set.
    /// </summary>
    public class Z80Instruction
    {
        public Z80Instruction(ushort offset, Z80OpCode opCode, byte[] operand)
        {
            Offset = offset;
            OpCode = opCode;
            RawOperand = operand;
        }

        public Z80Instruction(Z80OpCode opCode, byte[] operand)
            : this(0, opCode, operand)
        {
        }

        /// <summary>
        /// The memory address the instruction is located at.
        /// </summary>
        public ushort Offset
        {
            get;
        }

        /// <summary>
        /// The operation code of the instructon.
        /// </summary>
        public Z80OpCode OpCode
        {
            get;
        }

        /// <summary>
        /// The bytes that form the operand of the instruction.
        /// </summary>
        public byte[] RawOperand
        {
            get;
        }

        /// <summary>
        /// The operand interpreted as a single unsigned 8 bit integer.
        /// </summary>
        public byte Operand8
        {
            get { return RawOperand[0]; }
        }

        /// <summary>
        /// The operand interpreted as an unsigned 16 bit integer.
        /// </summary>
        public ushort Operand16
        {
            get { return BitConverter.ToUInt16(RawOperand, 0); }
        }

        public override string ToString()
        {
            switch (RawOperand.Length)
            {
                default:
                    return Offset.ToString("X4") + ": " + OpCode.Disassembly;
                case 1:
                    return Offset.ToString("X4") + ": " + string.Format(OpCode.Disassembly, Operand8);
                case 2:
                    return Offset.ToString("X4") + ": " + string.Format(OpCode.Disassembly, Operand16);
            }
        }

        /// <summary>
        /// Executes the instruction on the given device.
        /// </summary>
        /// <param name="device">The device to execute the instruction on.</param>
        /// <returns>The clock cycles it took to evaluate the instruction.</returns>
        public int Execute(GameBoy device)
        {
            return OpCode.Operation(device, this);
        }
    }
}
