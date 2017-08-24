﻿using System;
using System.Collections.Generic;

namespace Emux.GameBoy.Audio
{
    public class GameBoySpu
    {
        private readonly GameBoy _device;
        private readonly byte[] _unused = new byte[9];

        public GameBoySpu(GameBoy device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));
            _device = device;
            var channels = new List<ISoundChannel>();
            channels.Add(null);
            channels.Add(null);
            channels.Add(Wave = new WaveSoundChannel());
            channels.Add(null);
            Channels = channels.AsReadOnly();
        }

        public byte NR51
        {
            get;
            set;
        }

        public byte NR52
        {
            get;
            set;
        }

        public byte NR53
        {
            get;
            set;
        }

        public IAudioOutput AudioOutput
        {
            get;
            set;
        }

        public IList<ISoundChannel> Channels
        {
            get;
        }

        public WaveSoundChannel Wave
        {
            get;
        }
        
        public void WriteRegister(ushort address, byte value)
        {
            switch (address)
            {
                case 0xFF24:
                    NR51 = value;
                    break;
                case 0xFF25:
                    NR52 = value;
                    break;
                case 0xFF26:
                    NR53 = value;
                    break;
                default:
                    if (address >= 0xFF27 && address < 0xFF30)
                    {
                        _unused[address - 0xFF27] = value;
                    }
                    else if (address >= 0xFF30 && address < 0xFF40)
                    {
                        Wave.WriteWavRam((ushort) (address - 0xFF30), value);
                    }
                    else
                    {
                        int relativeAddress = address - 0xFF10;
                        int channelIndex = relativeAddress / 5;
                        var channel = Channels[channelIndex];

                        // TODO: remove
                        if (channel == null)
                            return;

                        switch (relativeAddress % 5)
                        {
                            case 0:
                                channel.NR0 = value;
                                break;
                            case 1:
                                channel.NR1 = value;
                                break;
                            case 2:
                                channel.NR2 = value;
                                break;
                            case 3:
                                channel.NR3 = value;
                                break;
                            case 4:
                                channel.NR4 = value;
                                break;
                        }
                    }
                    break;
            }
        }

        public byte ReadRegister(ushort address)
        {
            switch (address)
            {
                case 0xFF24:
                    return NR51;
                case 0xFF25:
                    return NR52;
                case 0xFF26:
                    return NR53;
                default:
                    if (address >= 0xFF27 && address < 0xFF30)
                        return _unused[address - 0xFF27];
                    if (address >= 0xFF30 && address < 0xFF40)
                        return Wave.ReadWavRam((ushort) (address - 0xFF30));

                    int relativeAddress = address - 0xFF10;
                    int channelIndex = relativeAddress / 5;
                    var channel = Channels[channelIndex];

                    // TODO: remove
                    if (channel == null)
                        return 0;

                    switch (relativeAddress % 5)
                    {
                        case 0:
                            return channel.NR0;
                        case 1:
                            return channel.NR1;
                        case 2:
                            return channel.NR2;
                        case 3:
                            return channel.NR3;
                        case 4:
                            return channel.NR4;
                    }
                    throw new ArgumentOutOfRangeException(nameof(address));
            }
        }
    }
}
