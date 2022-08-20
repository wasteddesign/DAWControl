using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Buzz.MachineInterface;
using BuzzGUI.Interfaces;
using BuzzGUI.Common;
using System.ComponentModel;

/*
 * Rules for adding new binding.
 * 
 * 1. Add new property to class State
 * 2. Add new enumeration to DAWControlType
 * 3. Update void UpdateTypeData()
 * 4. Update UpdateChannelData()
 * 5. Update UpdateNoteValueData()
 * 6. Update UpdateCCData()
 * 7. Update AssignDAWItem and DAWControlGUI()
 * 8. Update MidiControlChange()
 * Pretty much ta-da!
 * 
 */

namespace WDE.DAWControl
{
    [MachineDecl(Name = "DAW Control", ShortName = "DAW CTRL", Author = "WDE", MaxTracks = 1)]
	public class DAWControlMachine : IBuzzMachine
	{
		IBuzzMachineHost host;
        DAWControlGUI gui;
        DAWControlType AutoAssign = DAWControlType.None;

        public int JumpTriggerCounter { get; private set; }
        public bool SetJump { get; private set; }
        public int JumpTargetPosition { get; private set; }
        public int JumpTriggerTick { get; private set; }

        public DAWControlMachine(IBuzzMachineHost host)
		{
			this.host = host;            

            ResetJump();

            Global.Buzz.Song.MachineRemoved += Song_MachineRemoved;
            Global.Buzz.PropertyChanged += Buzz_PropertyChanged;
        }

        void ResetJump()
        {
            JumpTargetPosition = -1;
            JumpTriggerCounter = -1;
            JumpTriggerTick = 4;
            SetJump = false;
        }

        private void Buzz_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Playing")
            {
                if (!Global.Buzz.Playing)
                {
                    ResetJump();
                }
            }
        }

        private void Song_MachineRemoved(IMachine obj)
        {
            if (host.Machine == obj)
            {
                Global.Buzz.PropertyChanged -= Buzz_PropertyChanged;
                Global.Buzz.Song.MachineRemoved -= Song_MachineRemoved;
            }
        }
        

        public void SomethingChanged()
        {
            int val = host.Machine.ParameterGroups[0].Parameters[0].GetValue(0);
            host.Machine.ParameterGroups[0].Parameters[0].SetValue(0, val);
        }

        public void MidiControlChange(int ctrl, int channel, int value)
        {
            if (AutoAssign != DAWControlType.None && (value == 127))
            {
                switch (AutoAssign)
                {
                    case DAWControlType.Play:
                        MachineState.Play.MidiChannel = channel + 1;
                        MachineState.Play.MidiCC = ctrl;
                        break;
                    case DAWControlType.Stop:
                        MachineState.Stop.MidiChannel = channel + 1;
                        MachineState.Stop.MidiCC = ctrl;
                        break;
                    case DAWControlType.Record:
                        MachineState.Rec.MidiChannel = channel + 1;
                        MachineState.Rec.MidiCC = ctrl;
                        break;
                    case DAWControlType.Forward:
                        MachineState.Forward.MidiChannel = channel + 1;
                        MachineState.Forward.MidiCC = ctrl;
                        break;
                    case DAWControlType.Rewind:
                        MachineState.Rewind.MidiChannel = channel + 1;
                        MachineState.Rewind.MidiCC = ctrl;
                        break;
                    case DAWControlType.LoopStart:
                        MachineState.LoopStart.MidiChannel = channel + 1;
                        MachineState.LoopStart.MidiCC = ctrl;
                        break;
                    case DAWControlType.Solo:
                        MachineState.SoloMachine.MidiChannel = channel + 1;
                        MachineState.SoloMachine.MidiCC = ctrl;
                        break;
                    case DAWControlType.Solo2:
                        MachineState.SoloMachine2.MidiChannel = channel + 1;
                        MachineState.SoloMachine2.MidiCC = ctrl;
                        break;
                    case DAWControlType.Solo3:
                        MachineState.SoloMachine3.MidiChannel = channel + 1;
                        MachineState.SoloMachine3.MidiCC = ctrl;
                        break;
                    case DAWControlType.Solo4:
                        MachineState.SoloMachine4.MidiChannel = channel + 1;
                        MachineState.SoloMachine4.MidiCC = ctrl;
                        break;
                    case DAWControlType.Mute:
                        MachineState.MuteMachine.MidiChannel = channel + 1;
                        MachineState.MuteMachine.MidiCC = ctrl;
                        break;
                    case DAWControlType.Mute2:
                        MachineState.MuteMachine2.MidiChannel = channel + 1;
                        MachineState.MuteMachine2.MidiCC = ctrl;
                        break;
                    case DAWControlType.Mute3:
                        MachineState.MuteMachine3.MidiChannel = channel + 1;
                        MachineState.MuteMachine3.MidiCC = ctrl;
                        break;
                    case DAWControlType.Mute4:
                        MachineState.MuteMachine4.MidiChannel = channel + 1;
                        MachineState.MuteMachine4.MidiCC = ctrl;
                        break;
                    case DAWControlType.LoopToggle:
                        MachineState.LoopToggle.MidiChannel = channel + 1;
                        MachineState.LoopToggle.MidiCC = ctrl;
                        break;
                    case DAWControlType.IncreaseSpeed:
                        MachineState.IncreaseSpeed.MidiChannel = channel + 1;
                        MachineState.IncreaseSpeed.MidiCC = ctrl;
                        break;
                    case DAWControlType.DecreaseSpeed:
                        MachineState.DecreaseSpeed.MidiChannel = channel + 1;
                        MachineState.DecreaseSpeed.MidiCC = ctrl;
                        break;
                    case DAWControlType.Jump1:
                        MachineState.Jump1.MidiChannel = channel + 1;
                        MachineState.Jump1.MidiCC = ctrl;
                        break;
                    case DAWControlType.Jump2:
                        MachineState.Jump2.MidiChannel = channel + 1;
                        MachineState.Jump2.MidiCC = ctrl;
                        break;
                    case DAWControlType.Jump3:
                        MachineState.Jump3.MidiChannel = channel + 1;
                        MachineState.Jump3.MidiCC = ctrl;
                        break;
                    case DAWControlType.Jump4:
                        MachineState.Jump4.MidiChannel = channel + 1;
                        MachineState.Jump4.MidiCC = ctrl;
                        break;
                }
                if (gui != null)
                {
                    DAWControlType aType = AutoAssign;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {                        
                        gui.UpdateValues(aType);
                        gui.DisableAutoAssign();
                    }));                    
                }
                AutoAssign = DAWControlType.None;
                SomethingChanged();
            }
            else
            {
                if ((MachineState.Play.MidiChannel == channel + 1) && (MachineState.Play.MidiCC == ctrl) && (value == 127))
                    Play();
                else if ((MachineState.Stop.MidiChannel == channel + 1) && (MachineState.Stop.MidiCC == ctrl) && (value == 127))
                    StopSong();
                else if ((MachineState.Rec.MidiChannel== channel + 1) && (MachineState.Rec.MidiCC == ctrl) && (value == 127))
                    Record();
                else if ((MachineState.Forward.MidiChannel == channel + 1) && (MachineState.Forward.MidiCC == ctrl) && (value == 127))
                    Forward();
                else if ((MachineState.Rewind.MidiChannel == channel + 1) && (MachineState.Rewind.MidiCC == ctrl) && (value == 127))
                    Rewind();
                else if ((MachineState.LoopStart.MidiChannel == channel + 1) && (MachineState.LoopStart.MidiCC == ctrl) && (value == 127))
                    LoopStart();
                else if ((MachineState.SoloMachine.MidiChannel == channel + 1) && (MachineState.SoloMachine.MidiCC == ctrl) && (value == 127))
                    ToggleSolo(MachineState.SoloMachine.TargetMachineName);
                else if ((MachineState.SoloMachine2.MidiChannel == channel + 1) && (MachineState.SoloMachine2.MidiCC == ctrl) && (value == 127))
                    ToggleSolo(MachineState.SoloMachine2.TargetMachineName);
                else if ((MachineState.SoloMachine3.MidiChannel == channel + 1) && (MachineState.SoloMachine3.MidiCC == ctrl) && (value == 127))
                    ToggleSolo(MachineState.SoloMachine3.TargetMachineName);
                else if ((MachineState.SoloMachine4.MidiChannel == channel + 1) && (MachineState.SoloMachine4.MidiCC == ctrl) && (value == 127))
                    ToggleSolo(MachineState.SoloMachine4.TargetMachineName);
                else if ((MachineState.MuteMachine.MidiChannel == channel + 1) && (MachineState.MuteMachine.MidiCC == ctrl) && (value == 127))
                    ToggleMute(MachineState.MuteMachine.TargetMachineName);
                else if ((MachineState.MuteMachine2.MidiChannel == channel + 1) && (MachineState.MuteMachine2.MidiCC == ctrl) && (value == 127))
                    ToggleMute(MachineState.MuteMachine2.TargetMachineName);
                else if ((MachineState.MuteMachine3.MidiChannel == channel + 1) && (MachineState.MuteMachine3.MidiCC == ctrl) && (value == 127))
                    ToggleMute(MachineState.MuteMachine3.TargetMachineName);
                else if ((MachineState.MuteMachine4.MidiChannel == channel + 1) && (MachineState.MuteMachine4.MidiCC == ctrl) && (value == 127))
                    ToggleMute(MachineState.MuteMachine4.TargetMachineName);
                else if ((MachineState.LoopToggle.MidiChannel == channel + 1) && (MachineState.LoopToggle.MidiCC == ctrl) && (value == 127))
                    ToggleLoop();
                else if ((MachineState.IncreaseSpeed.MidiChannel == channel + 1) && (MachineState.IncreaseSpeed.MidiCC == ctrl) && (value == 127))
                    IncreaseSpeed();
                else if ((MachineState.DecreaseSpeed.MidiChannel == channel + 1) && (MachineState.DecreaseSpeed.MidiCC == ctrl) && (value == 127))
                    DecreaseSpeed();
                else if ((MachineState.Jump1.MidiChannel == channel + 1) && (MachineState.Jump1.MidiCC == ctrl) && (value == 127))
                    Jump(MachineState.Jump1.TargetPos, MachineState.Jump1.TriggerTick);
                else if ((MachineState.Jump2.MidiChannel == channel + 1) && (MachineState.Jump2.MidiCC == ctrl) && (value == 127))
                    Jump(MachineState.Jump2.TargetPos, MachineState.Jump2.TriggerTick);
                else if ((MachineState.Jump3.MidiChannel == channel + 1) && (MachineState.Jump3.MidiCC == ctrl) && (value == 127))
                    Jump(MachineState.Jump3.TargetPos, MachineState.Jump3.TriggerTick);
                else if ((MachineState.Jump4.MidiChannel == channel + 1) && (MachineState.Jump4.MidiCC == ctrl) && (value == 127))
                    Jump(MachineState.Jump4.TargetPos, MachineState.Jump4.TriggerTick);
            }

            if (gui != null)
                gui.MidiControlChange(ctrl, channel, value);            
        }

        private void Jump(int targetPos, int triggerTick)
        {
            // Active jump target
            JumpTargetPosition = targetPos;
            JumpTriggerTick = triggerTick;
            SetJump = true;
        }

        public void MidiNote(int channel, int value, int velocity)
        {
            if (AutoAssign != DAWControlType.None && (velocity == 127))
            {
                switch (AutoAssign)
                {
                    case DAWControlType.Play:
                        MachineState.Play.MidiChannel = channel + 1;
                        MachineState.Play.MidiNoteValue = value;
                        break;
                    case DAWControlType.Stop:
                        MachineState.Stop.MidiChannel = channel + 1;
                        MachineState.Stop.MidiNoteValue = value;
                        break;
                    case DAWControlType.Record:
                        MachineState.Rec.MidiChannel = channel + 1;
                        MachineState.Rec.MidiNoteValue = value;
                        break;
                    case DAWControlType.Forward:
                        MachineState.Forward.MidiChannel = channel + 1;
                        MachineState.Forward.MidiNoteValue = value;
                        break;
                    case DAWControlType.Rewind:
                        MachineState.Rewind.MidiChannel = channel + 1;
                        MachineState.Rewind.MidiNoteValue = value;
                        break;
                    case DAWControlType.LoopStart:
                        MachineState.LoopStart.MidiChannel = channel + 1;
                        MachineState.LoopStart.MidiNoteValue = value;
                        break;
                    case DAWControlType.Solo:
                        MachineState.SoloMachine.MidiChannel = channel + 1;
                        MachineState.SoloMachine.MidiNoteValue = value;
                        break;
                    case DAWControlType.Solo2:
                        MachineState.SoloMachine2.MidiChannel = channel + 1;
                        MachineState.SoloMachine2.MidiNoteValue = value;
                        break;
                    case DAWControlType.Solo3:
                        MachineState.SoloMachine3.MidiChannel = channel + 1;
                        MachineState.SoloMachine3.MidiNoteValue = value;
                        break;
                    case DAWControlType.Solo4:
                        MachineState.SoloMachine4.MidiChannel = channel + 1;
                        MachineState.SoloMachine4.MidiNoteValue = value;
                        break;
                    case DAWControlType.Mute:
                        MachineState.MuteMachine.MidiChannel = channel + 1;
                        MachineState.MuteMachine.MidiNoteValue = value;
                        break;
                    case DAWControlType.Mute2:
                        MachineState.MuteMachine2.MidiChannel = channel + 1;
                        MachineState.MuteMachine2.MidiNoteValue = value;
                        break;
                    case DAWControlType.Mute3:
                        MachineState.MuteMachine3.MidiChannel = channel + 1;
                        MachineState.MuteMachine3.MidiNoteValue = value;
                        break;
                    case DAWControlType.Mute4:
                        MachineState.MuteMachine4.MidiChannel = channel + 1;
                        MachineState.MuteMachine4.MidiNoteValue = value;
                        break;
                    case DAWControlType.LoopToggle:
                        MachineState.LoopToggle.MidiChannel = channel + 1;
                        MachineState.LoopToggle.MidiNoteValue = value;
                        break;
                    case DAWControlType.IncreaseSpeed:
                        MachineState.IncreaseSpeed.MidiChannel = channel + 1;
                        MachineState.IncreaseSpeed.MidiNoteValue = value;
                        break;
                    case DAWControlType.DecreaseSpeed:
                        MachineState.DecreaseSpeed.MidiChannel = channel + 1;
                        MachineState.DecreaseSpeed.MidiNoteValue = value;
                        break;
                    case DAWControlType.Jump1:
                        MachineState.Jump1.MidiChannel = channel + 1;
                        MachineState.Jump1.MidiNoteValue = value;
                        break;
                    case DAWControlType.Jump2:
                        MachineState.Jump2.MidiChannel = channel + 1;
                        MachineState.Jump2.MidiNoteValue = value;
                        break;
                    case DAWControlType.Jump3:
                        MachineState.Jump3.MidiChannel = channel + 1;
                        MachineState.Jump3.MidiNoteValue = value;
                        break;
                    case DAWControlType.Jump4:
                        MachineState.Jump4.MidiChannel = channel + 1;
                        MachineState.Jump4.MidiNoteValue = value;
                        break;

                }
                if (gui != null)
                {
                    DAWControlType aType = AutoAssign;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        gui.UpdateValues(aType);
                        gui.DisableAutoAssign();
                    }));
                }

                AutoAssign = DAWControlType.None;
                SomethingChanged();
            }
            else
            {
                if ((MachineState.Play.MidiChannel == channel + 1) && (MachineState.Play.MidiNoteValue == value) && (velocity == 127))
                    Play();
                else if ((MachineState.Stop.MidiChannel == channel + 1) && (MachineState.Stop.MidiNoteValue == value) && (velocity == 127))
                    StopSong();
                else if ((MachineState.Rec.MidiChannel == channel + 1) && (MachineState.Rec.MidiNoteValue == value) && (velocity == 127))
                    Record();
                else if ((MachineState.Forward.MidiChannel == channel + 1) && (MachineState.Forward.MidiNoteValue == value) && (velocity == 127))
                    Forward();
                else if ((MachineState.Rewind.MidiChannel == channel + 1) && (MachineState.Rewind.MidiNoteValue == value) && (velocity == 127))
                    Rewind();
                else if ((MachineState.LoopStart.MidiChannel == channel + 1) && (MachineState.LoopStart.MidiNoteValue == value) && (velocity == 127))
                    LoopStart();
                else if ((MachineState.SoloMachine.MidiChannel == channel + 1) && (MachineState.SoloMachine.MidiNoteValue == value) && (velocity == 127))
                    ToggleSolo(MachineState.SoloMachine.TargetMachineName);
                else if ((MachineState.SoloMachine2.MidiChannel == channel + 1) && (MachineState.SoloMachine2.MidiNoteValue == value) && (velocity == 127))
                    ToggleSolo(MachineState.SoloMachine2.TargetMachineName);
                else if ((MachineState.SoloMachine3.MidiChannel == channel + 1) && (MachineState.SoloMachine3.MidiNoteValue == value) && (velocity == 127))
                    ToggleSolo(MachineState.SoloMachine3.TargetMachineName);
                else if ((MachineState.SoloMachine4.MidiChannel == channel + 1) && (MachineState.SoloMachine4.MidiNoteValue == value) && (velocity == 127))
                    ToggleSolo(MachineState.SoloMachine4.TargetMachineName);
                else if ((MachineState.MuteMachine.MidiChannel == channel + 1) && (MachineState.MuteMachine.MidiNoteValue == value) && (velocity == 127))
                    ToggleMute(MachineState.MuteMachine.TargetMachineName);
                else if ((MachineState.MuteMachine2.MidiChannel == channel + 1) && (MachineState.MuteMachine2.MidiNoteValue == value) && (velocity == 127))
                    ToggleMute(MachineState.MuteMachine2.TargetMachineName);
                else if ((MachineState.MuteMachine3.MidiChannel == channel + 1) && (MachineState.MuteMachine3.MidiNoteValue == value) && (velocity == 127))
                    ToggleMute(MachineState.MuteMachine3.TargetMachineName);
                else if ((MachineState.MuteMachine4.MidiChannel == channel + 1) && (MachineState.MuteMachine4.MidiNoteValue == value) && (velocity == 127))
                    ToggleMute(MachineState.MuteMachine4.TargetMachineName);
                else if ((MachineState.LoopToggle.MidiChannel == channel + 1) && (MachineState.LoopToggle.MidiNoteValue == value) && (velocity == 127))
                    ToggleLoop();
                else if ((MachineState.IncreaseSpeed.MidiChannel == channel + 1) && (MachineState.IncreaseSpeed.MidiNoteValue == value) && (velocity == 127))
                    IncreaseSpeed();
                else if ((MachineState.DecreaseSpeed.MidiChannel == channel + 1) && (MachineState.DecreaseSpeed.MidiNoteValue == value) && (velocity == 127))
                    DecreaseSpeed();
                else if ((MachineState.Jump1.MidiChannel == channel + 1) && (MachineState.Jump1.MidiNoteValue == value) && (velocity == 127))
                    Jump(MachineState.Jump1.TargetPos, MachineState.Jump1.TriggerTick);
                else if ((MachineState.Jump2.MidiChannel == channel + 1) && (MachineState.Jump2.MidiNoteValue == value) && (velocity == 127))
                    Jump(MachineState.Jump2.TargetPos, MachineState.Jump2.TriggerTick);
                else if ((MachineState.Jump3.MidiChannel == channel + 1) && (MachineState.Jump3.MidiNoteValue == value) && (velocity == 127))
                    Jump(MachineState.Jump3.TargetPos, MachineState.Jump3.TriggerTick);
                else if ((MachineState.Jump4.MidiChannel == channel + 1) && (MachineState.Jump4.MidiNoteValue == value) && (velocity == 127))
                    Jump(MachineState.Jump4.TargetPos, MachineState.Jump4.TriggerTick);
            }
            if (gui != null)
                gui.MidiNote(channel, value, velocity);
        }

        public void SetGUI(DAWControlGUI gui)
        {
            this.gui = gui;
        }
        
        [ParameterDecl(ValueDescriptions = new[] { "no", "yes" })]
		public bool Bypass { get; set; }
                    
        public void Work() 
        {
            if (SetJump)
            {
                SetJump = false;
                int playPos = host.Machine.Graph.Buzz.Song.PlayPosition;
                
                JumpTriggerCounter = JumpTriggerTick - playPos % JumpTriggerTick - 2 + host.Machine.Graph.Buzz.Song.LoopStart % JumpTriggerTick;
                JumpTriggerCounter = JumpTriggerCounter < 0 ? JumpTriggerCounter + JumpTriggerTick : JumpTriggerCounter;
                // Global.Buzz.DCWriteLine("!! Reset !! PosinTick: " + host.MasterInfo.PosInTick + " | PlayPos: " + host.Machine.Graph.Buzz.Song.PlayPosition + " | JumpTriggerTick: " + JumpTriggerTick + " | JumpTriggerCounter: " + JumpTriggerCounter);
                
            }

            if (host.MasterInfo.PosInTick == 0 && host.SubTickInfo.PosInSubTick == 0 && Global.Buzz.Playing)
            {
                if (JumpTargetPosition != -1)
                {
                    // Global.Buzz.DCWriteLine("PlayPos: " + host.Machine.Graph.Buzz.Song.PlayPosition + " | JumpTriggerCounter: " + JumpTriggerCounter);
                    if (JumpTriggerCounter == 0)
                    {
                        host.Machine.Graph.Buzz.Song.PlayPosition = JumpTargetPosition;
                        JumpTargetPosition = -1;
                    }
                    JumpTriggerCounter--;
                }
            }
        }
        
        public void Play()
        {
            host.Machine.Graph.Buzz.Playing = !host.Machine.Graph.Buzz.Playing;
        }

        public void StopSong()
        {
            host.Machine.Graph.Buzz.Playing = false;
        }

        public void Record()
        {
            host.Machine.Graph.Buzz.Recording = true;            
        }

        public void IncreaseSpeed()
        {
            host.Machine.Graph.Buzz.Speed += 1;            
        }

        public void DecreaseSpeed()
        {
            host.Machine.Graph.Buzz.Speed -= 1;
        }

        public void Rewind()
        {
            host.Machine.Graph.Buzz.Song.PlayPosition -= 4;
        }

        public void Forward()
        {
            host.Machine.Graph.Buzz.Song.PlayPosition += 4;
        }

        public void LoopSong()
        {
            host.Machine.Graph.Buzz.Looping = !host.Machine.Graph.Buzz.Looping;
        }

        public void LoopStart()
        {
            host.Machine.Graph.Buzz.Song.PlayPosition = host.Machine.Graph.Buzz.Song.LoopStart;
        }

        private void ToggleLoop()
        {
            host.Machine.Graph.Buzz.Looping = !host.Machine.Graph.Buzz.Looping;
        }

        private void ToggleSolo(string machineName)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var m in host.Machine.Graph.Machines)
                if (m.Name == machineName)
                {
                    m.IsSoloed = !m.IsSoloed;
                    break;
                }
            }));
        }

        private void ToggleMute(string machineName)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var m in host.Machine.Graph.Machines)
                    if (m.Name == machineName)
                    {
                        m.IsMuted = !m.IsMuted;
                        break;
                    }
            }));
        }

        public class MidiData
        {
            int midiChannel;
            int midiNoteValue;
            int midiCC;
            string targetMahicneName;
            int triggerTick = 4;
            int targetPos;

            public int MidiChannel { get => midiChannel; set => midiChannel = value; }
            public int MidiNoteValue { get => midiNoteValue; set => midiNoteValue = value; }
            public int MidiCC { get => midiCC; set => midiCC = value; }
            public string TargetMachineName { get => targetMahicneName; set => targetMahicneName = value; }
            public int TriggerTick { get => triggerTick; set => triggerTick = value; }
            public int TargetPos { get => targetPos; set => targetPos = value; }
        }

        // actual machine ends here. the stuff below demonstrates some other features of the api.
        public class State
		{
			public State() {                
                loopToggle = new MidiData();
                soloMachine = new MidiData();
                soloMachine2 = new MidiData();
                soloMachine3 = new MidiData();
                soloMachine4 = new MidiData();
                muteMachine = new MidiData();
                muteMachine2 = new MidiData();
                muteMachine3 = new MidiData();
                muteMachine4 = new MidiData();
                loopStart = new MidiData();
                rewind = new MidiData();
                forward = new MidiData();
                rec = new MidiData();
                stop = new MidiData();
                play = new MidiData();
                increaseSpeed = new MidiData();
                decreaseSpeed = new MidiData();
                jump1 = new MidiData();
                jump2 = new MidiData();
                jump3 = new MidiData();
                jump4 = new MidiData();
            }	// NOTE: parameterless constructor is required by the xml serializer

            public MidiData LoopToggle { get => loopToggle; set => loopToggle = value; }
            public MidiData SoloMachine { get => soloMachine; set => soloMachine = value; }
            public MidiData SoloMachine2 { get => soloMachine2; set => soloMachine2 = value; }
            public MidiData SoloMachine3 { get => soloMachine3; set => soloMachine3 = value; }
            public MidiData SoloMachine4 { get => soloMachine4; set => soloMachine4 = value; }
            public MidiData MuteMachine { get => muteMachine; set => muteMachine = value; }
            public MidiData MuteMachine2 { get => muteMachine2; set => muteMachine2 = value; }
            public MidiData MuteMachine3 { get => muteMachine3; set => muteMachine3 = value; }
            public MidiData MuteMachine4 { get => muteMachine4; set => muteMachine4 = value; }
            public MidiData LoopStart { get => loopStart; set => loopStart = value; }
            public MidiData Rewind { get => rewind; set => rewind = value; }
            public MidiData Forward { get => forward; set => forward = value; }
            public MidiData Rec { get => rec; set => rec = value; }
            public MidiData Stop { get => stop; set => stop = value; }
            public MidiData Play { get => play; set => play = value; }
            public MidiData IncreaseSpeed { get => increaseSpeed; set => increaseSpeed = value; }
            public MidiData DecreaseSpeed { get => decreaseSpeed; set => decreaseSpeed = value; }

            public MidiData Jump1 { get => jump1; set => jump1 = value; }
            public MidiData Jump2 { get => jump2; set => jump2 = value; }
            public MidiData Jump3 { get => jump3; set => jump3 = value; }
            public MidiData Jump4 { get => jump4; set => jump4 = value; }

            MidiData play;
            MidiData stop;
            MidiData rec;
            MidiData rewind;
            MidiData forward;
            MidiData loopStart;
            MidiData loopToggle;
            MidiData soloMachine;
            MidiData soloMachine2;
            MidiData soloMachine3;
            MidiData soloMachine4;
            MidiData increaseSpeed;
            MidiData decreaseSpeed;
            MidiData muteMachine;
            MidiData muteMachine2;
            MidiData muteMachine3;
            MidiData muteMachine4;
            MidiData jump1;
            MidiData jump2;
            MidiData jump3;
            MidiData jump4;
        }

		State machineState = new State();
		public State MachineState			// a property called 'MachineState' gets automatically saved in songs and presets
		{
			get { return machineState; }
			set
			{
				machineState = value;				
			}
		}		
					
		public IEnumerable<IMenuItem> Commands
		{
			get
			{
				yield return new MenuItemVM() 
				{ 
					Text = "About...", 
					Command = new SimpleCommand()
					{
						CanExecuteDelegate = p => true,
						ExecuteDelegate = p => MessageBox.Show(
@"DAW Control 1.2 (C) 2021 WDE

Control Buzz using MIDI controller.")
					}
				};
			}
		}

       

        internal void DisableAutoAssign()
        {
            AutoAssign = DAWControlType.None;            
        }

        internal void EnableAutoAssign(DAWControlType type)
        {
            AutoAssign = type;
        }

        internal IBuzzMachineHost getHost()
        {
            return host;
        }
    }

    public enum DAWControlType
    {
        Play,
        Stop,
        Record,
        Rewind,
        Forward,
        LoopStart,
        None,
        Solo,
        Solo2,
        Solo3,
        Solo4,
        LoopToggle,
        SetLoopStart,
        SetLoopEnd,
        IncreaseSpeed,
        DecreaseSpeed,
        Mute,
        Mute2,
        Mute3,
        Mute4,
        Jump1,
        Jump2,
        Jump3,
        Jump4,
    }

	public class MachineGUIFactory : IMachineGUIFactory { public IMachineGUI CreateGUI(IMachineGUIHost host) { return new DAWControlGUI(); } }
    public class DAWControlGUI : UserControl, IMachineGUI
    {
        IMachine machine;
        DAWControlMachine dawMachine;

        ListBox lb;
        AssignDAWItem itemPlay;
        AssignDAWItem itemStop;
        AssignDAWItem itemRecord;
        AssignDAWItem itemForward;
        AssignDAWItem itemRewind;
        AssignDAWItem itemLoopStart;
        AssignDAWItem itemSolo;
        AssignDAWItem itemSolo2;
        AssignDAWItem itemSolo3;
        AssignDAWItem itemSolo4;
        AssignDAWItem itemMute;
        AssignDAWItem itemMute2;
        AssignDAWItem itemMute3;
        AssignDAWItem itemMute4;
        AssignDAWItem itemToggleLoop;
        AssignDAWItem itemIncreaseSpeed;
        AssignDAWItem itemDecreaseSpeed;
        AssignDAWItem itemJump1;
        AssignDAWItem itemJump2;
        AssignDAWItem itemJump3;
        AssignDAWItem itemJump4;
        StackPanel spMain;
        RadioButton rb;
                
        public class MachineVM
        {
            public IMachine Machine { get; private set; }
            public MachineVM(IMachine m) { Machine = m; }
            public override string ToString() { return Machine.Name; }
        }

        public IMachine Machine
        {
            get { return machine; }
            set
            {
                if (machine != null)
                {
                    itemSolo.RemoveEvents();
                }

                machine = value;

                if (machine != null)
                {
                    dawMachine = (DAWControlMachine)machine.ManagedMachine;
                    itemPlay.SetMachine(dawMachine, this);
                    itemStop.SetMachine(dawMachine, this);
                    itemRecord.SetMachine(dawMachine, this);
                    itemForward.SetMachine(dawMachine, this);
                    itemRewind.SetMachine(dawMachine, this);
                    itemLoopStart.SetMachine(dawMachine, this);
                    itemSolo.SetMachine(dawMachine, this);
                    itemSolo2.SetMachine(dawMachine, this);
                    itemSolo3.SetMachine(dawMachine, this);
                    itemSolo4.SetMachine(dawMachine, this);
                    itemMute.SetMachine(dawMachine, this);
                    itemMute2.SetMachine(dawMachine, this);
                    itemMute3.SetMachine(dawMachine, this);
                    itemMute4.SetMachine(dawMachine, this);
                    itemToggleLoop.SetMachine(dawMachine, this);
                    itemIncreaseSpeed.SetMachine(dawMachine, this);
                    itemDecreaseSpeed.SetMachine(dawMachine, this);
                    itemJump1.SetMachine(dawMachine, this);
                    itemJump2.SetMachine(dawMachine, this);
                    itemJump3.SetMachine(dawMachine, this);
                    itemJump4.SetMachine(dawMachine, this);

                    dawMachine.SetGUI(this);                    
                }
            }
        }
         
        public void MidiNote(int channel, int value, int velocity)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                lb.Items.Add("MIDI Note | Channel: " + (channel + 1) + ", Note Value: " + value + ", Velocity: " + velocity);
                if (lb.Items.Count > 20)
                    lb.Items.RemoveAt(0);
            }));
        }

        internal void MidiControlChange(int ctrl, int channel, int value)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                lb.Items.Add("MIDI Control Change | Channel: " + (channel + 1) + ", Control: " + ctrl + ", Value: " + value);
                if (lb.Items.Count > 20)
                    lb.Items.RemoveAt(0);
            }));
        }

        public DAWControlGUI()
		{			
            itemPlay = new AssignDAWItem("Play", DAWControlType.Play);
            itemStop = new AssignDAWItem("Stop", DAWControlType.Stop);
            itemRecord = new AssignDAWItem("Record", DAWControlType.Record);
            itemForward = new AssignDAWItem("Forward", DAWControlType.Forward);
            itemRewind = new AssignDAWItem("Rewind", DAWControlType.Rewind);
            itemLoopStart = new AssignDAWItem("Goto Beginning", DAWControlType.LoopStart);
            itemSolo = new AssignDAWItem("Solo 1", DAWControlType.Solo);
            itemSolo2 = new AssignDAWItem("Solo 2", DAWControlType.Solo2);
            itemSolo3 = new AssignDAWItem("Solo 3", DAWControlType.Solo3);
            itemSolo4 = new AssignDAWItem("Solo 4", DAWControlType.Solo4);
            itemMute = new AssignDAWItem("Mute 1", DAWControlType.Mute);
            itemMute2 = new AssignDAWItem("Mute 2", DAWControlType.Mute2);
            itemMute3 = new AssignDAWItem("Mute 3", DAWControlType.Mute3);
            itemMute4 = new AssignDAWItem("Mute 4", DAWControlType.Mute4);
            itemToggleLoop = new AssignDAWItem("Loop Enabled", DAWControlType.LoopToggle);
            itemIncreaseSpeed = new AssignDAWItem("Increase Speed", DAWControlType.IncreaseSpeed);
            itemDecreaseSpeed = new AssignDAWItem("Decrease Speed", DAWControlType.DecreaseSpeed);
            itemJump1 = new AssignDAWItem("Jump 1", DAWControlType.Jump1);
            itemJump2 = new AssignDAWItem("Jump 2", DAWControlType.Jump2);
            itemJump3 = new AssignDAWItem("Jump 3", DAWControlType.Jump3);
            itemJump4 = new AssignDAWItem("Jump 4", DAWControlType.Jump4);

            lb = new ListBox() { Height = 200, Margin = new Thickness(0, 0, 0, 4) };
            lb.PreviewMouseRightButtonDown += (sender, e) =>
            {
                lb.Items.Clear();
            };

            var spMainLabel = new StackPanel();
            spMainLabel.Orientation = Orientation.Horizontal;
            Label labelName = new Label() { Content = "Target", Width = 100 };
            spMainLabel.Children.Add(labelName);
            Label labelChannel = new Label() { Content = "Channel", Width = 74 };
            spMainLabel.Children.Add(labelChannel);
            Label labelNoteValue = new Label() { Content = "Note", Width = 50 };
            spMainLabel.Children.Add(labelNoteValue);
            Label labelCC = new Label() { Content = "CC", Width = 60 };
            spMainLabel.Children.Add(labelCC);
            Label labelAuto = new Label() { Content = "Auto", Width = 40 };
            spMainLabel.Children.Add(labelAuto);

            spMain = new StackPanel() { } ;            
            
            spMain.Children.Add(spMainLabel);
			
            spMain.Children.Add(itemPlay);
            spMain.Children.Add(itemStop);
            spMain.Children.Add(itemRecord);
            spMain.Children.Add(itemForward);
            spMain.Children.Add(itemRewind);
            spMain.Children.Add(itemLoopStart);
            spMain.Children.Add(itemToggleLoop);
            spMain.Children.Add(itemIncreaseSpeed);
            spMain.Children.Add(itemDecreaseSpeed);
            spMain.Children.Add(itemSolo);
            spMain.Children.Add(itemSolo2);
            spMain.Children.Add(itemSolo3);
            spMain.Children.Add(itemSolo4);
            spMain.Children.Add(itemMute);
            spMain.Children.Add(itemMute2);
            spMain.Children.Add(itemMute3);
            spMain.Children.Add(itemMute4);

            var spJumpLabel = new StackPanel();
            spJumpLabel.Orientation = Orientation.Horizontal;
            Label labelEmpty = new Label() { Content = "", Width = 300 };
            spJumpLabel.Children.Add(labelEmpty);
            Label labelJump = new Label() { Content = "Jump to", Width = 54 };
            spJumpLabel.Children.Add(labelJump);
            Label labelTickSync = new Label() { Content = "Tick Sync", Width = 80 };
            spJumpLabel.Children.Add(labelTickSync);
            spMain.Children.Add(spJumpLabel);

            spMain.Children.Add(itemJump1);
            spMain.Children.Add(itemJump2);
            spMain.Children.Add(itemJump3);
            spMain.Children.Add(itemJump4);

            var spBottomLabel = new StackPanel();
            spBottomLabel.Orientation = Orientation.Horizontal;
            Label labelBottom = new Label() { Content = "Auto Assign Off", Width = 256, HorizontalContentAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 8, 0) };
            rb = new RadioButton() { GroupName = "AutoAssign", Margin = new Thickness(0, 6, 0, 0), IsChecked=true };
            rb.Checked += Rb_Checked;
            spBottomLabel.Children.Add(labelBottom);
            spBottomLabel.Children.Add(rb);
            
            spMain.Children.Add(spBottomLabel);
            spMain.Children.Add(lb);

            ScrollViewer sv = new ScrollViewer() { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, VerticalScrollBarVisibility= ScrollBarVisibility.Auto, Height=500 };
            sv.Content = spMain;
            this.Content = sv;

            this.Loaded += (sender, e) =>
            {
                dawMachine.getHost().Machine.ParameterWindow.MinWidth = 510;
            };
        }

        private void Rb_Checked(object sender, RoutedEventArgs e)
        {
            dawMachine.DisableAutoAssign();
        }

        internal void UpdateValues(DAWControlType type)
        {            
            switch (type)
            {
                case DAWControlType.Play:
                    itemPlay.UpdateTypeData();
                    break;
                case DAWControlType.Stop:
                    itemStop.UpdateTypeData();
                    break;
                case DAWControlType.Record:
                    itemRecord.UpdateTypeData();
                    break;
                case DAWControlType.Forward:
                    itemForward.UpdateTypeData();
                    break;
                case DAWControlType.Rewind:
                    itemRewind.UpdateTypeData();
                    break;
                case DAWControlType.LoopStart:
                    itemLoopStart.UpdateTypeData();
                    break;
                case DAWControlType.Solo:
                    itemSolo.UpdateTypeData();
                    break;
                case DAWControlType.Solo2:
                    itemSolo2.UpdateTypeData();
                    break;
                case DAWControlType.Solo3:
                    itemSolo3.UpdateTypeData();
                    break;
                case DAWControlType.Solo4:
                    itemSolo4.UpdateTypeData();
                    break;
                case DAWControlType.Mute:
                    itemMute.UpdateTypeData();
                    break;
                case DAWControlType.Mute2:
                    itemMute2.UpdateTypeData();
                    break;
                case DAWControlType.Mute3:
                    itemMute3.UpdateTypeData();
                    break;
                case DAWControlType.Mute4:
                    itemMute4.UpdateTypeData();
                    break;
                case DAWControlType.LoopToggle:
                    itemToggleLoop.UpdateTypeData();
                    break;
                case DAWControlType.IncreaseSpeed:
                    itemIncreaseSpeed.UpdateTypeData();
                    break;
                case DAWControlType.DecreaseSpeed:
                    itemDecreaseSpeed.UpdateTypeData();
                    break;
                case DAWControlType.Jump1:
                    itemJump1.UpdateTypeData();
                    break;
                case DAWControlType.Jump2:
                    itemJump2.UpdateTypeData();
                    break;
                case DAWControlType.Jump3:
                    itemJump3.UpdateTypeData();
                    break;
                case DAWControlType.Jump4:
                    itemJump4.UpdateTypeData();
                    break;
            }
        }

        internal void DisableAutoAssign()
        {
            rb.IsChecked = true;
        }
    }

}
