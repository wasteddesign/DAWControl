using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using static WDE.DAWControl.DAWControlGUI;
using BuzzGUI.Interfaces;
using System.Windows.Data;

namespace WDE.DAWControl
{
    public class AssignDAWItem : StackPanel
    {
        public string name;
        public ComboBox cbChannel;
        public TextBox tbNoteValue;
        public ComboBox cbCCValue;
        public Label label;

        public TextBox tbJumpTarget;
        public ComboBox cbTick;

        RadioButton rbAutoAssign;
        ComboBox cbMachineList;

        DAWControlMachine dawControlMachine;
        DAWControlGUI gui;
        public DAWControlType type;

        public ObservableCollection<MachineVM> Machines { get; private set; }

        public AssignDAWItem(string name, DAWControlType type)
        {
            this.name = name;
            this.type = type;

            this.Orientation = Orientation.Horizontal;
            cbChannel = new ComboBox() { BorderThickness = new Thickness(0, 0, 0, 0), Width = 68, VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 4, 4, 4) };
            cbChannel.Items.Add("Disabled");
            cbChannel.SelectedIndex = 0;
            for (int i = 0; i < 16; i++)
                cbChannel.Items.Add("" + (i + 1));
            cbChannel.SelectionChanged += CbChannel_SelectionChanged;

            tbNoteValue = new TextBox() { BorderThickness = new Thickness(0, 0, 0, 0), Text = "0", Width = 44, TextAlignment = TextAlignment.Right, MaxLength = 5 };
            tbNoteValue.PreviewTextInput += TbNoteValue_PreviewTextInput;
            tbNoteValue.SelectionChanged += TbNoteValue_SelectionChanged;
            cbCCValue = new ComboBox() { BorderThickness = new Thickness(0, 0, 0, 0), Width = 56, Margin = new Thickness(4, 4, 4, 4) };
            for (int i = 0; i < 128; i++)
                cbCCValue.Items.Add("" + (i));
            cbCCValue.SelectedIndex = 0;
            cbCCValue.SelectionChanged += CbCCValue_SelectionChanged;

            label = new Label() { Content = name, Width = 100 };

            rbAutoAssign = new RadioButton() { GroupName = "AutoAssign", Margin = new Thickness(4, 6, 4, 4) };
            rbAutoAssign.Checked += RbAutoAssign_Checked;
            rbAutoAssign.Unchecked += RbAutoAssign_Unchecked;

            this.Children.Add(label);
            this.Children.Add(cbChannel);
            this.Children.Add(tbNoteValue);
            this.Children.Add(cbCCValue);
            this.Children.Add(rbAutoAssign);

            if (type == DAWControlType.Solo || type == DAWControlType.Solo2 || type == DAWControlType.Solo3 || type == DAWControlType.Solo4 ||
                type == DAWControlType.Mute || type == DAWControlType.Mute2 || type == DAWControlType.Mute3 || type == DAWControlType.Mute4)
            {
                cbMachineList = new ComboBox() { BorderThickness = new Thickness(0, 0, 0, 0), Width = 150, Margin = new Thickness(4, 4, 4, 4) };
                this.Children.Add(cbMachineList);
                Machines = new ObservableCollection<MachineVM>();
                cbMachineList.SelectionChanged += CbMachineList_SelectionChanged;
            }

            if (type == DAWControlType.Jump1 || type == DAWControlType.Jump2 || type == DAWControlType.Jump3 || type == DAWControlType.Jump4 )
            {
                tbJumpTarget = new TextBox() { BorderThickness = new Thickness(0, 0, 0, 0), Text = "0", Width = 44, TextAlignment = TextAlignment.Right, MaxLength = 5 };
                this.Children.Add(tbJumpTarget);
                tbJumpTarget.PreviewTextInput += TbNoteValue_PreviewTextInput;
                tbJumpTarget.SelectionChanged += TbJumpTarget_SelectionChanged;
                cbTick = new ComboBox() { BorderThickness = new Thickness(0, 0, 0, 0), Width = 44, Margin = new Thickness(4, 4, 4, 4) };
                for (int i = 1; i < 32; i++)
                {
                    cbTick.Items.Add(i);
                }
                cbTick.SelectedIndex = 3; // 4
                cbTick.SelectionChanged += CbTick_SelectionChanged;
                this.Children.Add(cbTick);
            }
        }


        private void CbTick_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateJumpTick(type);
        }


        private void CbMachineList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedMachineData();
        }

        private void RbAutoAssign_Checked(object sender, RoutedEventArgs e)
        {
            dawControlMachine.EnableAutoAssign(type);
        }

        private void RbAutoAssign_Unchecked(object sender, RoutedEventArgs e)
        {
            dawControlMachine.DisableAutoAssign();
        }

        private void TbNoteValue_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void CbCCValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCCData();
        }

        private void TbNoteValue_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateNoteValueData();
        }

        private void CbChannel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateChannelData();
        }

        public void SetMachine(DAWControlMachine machine, DAWControlGUI gui)
        {
            this.dawControlMachine = machine;
            this.gui = gui;

            if (type == DAWControlType.Solo || type == DAWControlType.Solo2 || type == DAWControlType.Solo3 ||type == DAWControlType.Solo4 ||
                type == DAWControlType.Mute || type == DAWControlType.Mute2 || type == DAWControlType.Mute3 || type == DAWControlType.Mute4)
            {
                dawControlMachine.getHost().Machine.Graph.MachineAdded += Graph_MachineAdded;
                dawControlMachine.getHost().Machine.Graph.MachineRemoved += Graph_MachineRemoved;

                foreach (var m in dawControlMachine.getHost().Machine.Graph.Machines)
                    Graph_MachineAdded(m);

                cbMachineList.SetBinding(ComboBox.ItemsSourceProperty, new Binding("Machines") { Source = this, Mode = BindingMode.OneWay });
            }

            if (type == DAWControlType.Solo)
            {
                foreach (MachineVM mac in cbMachineList.Items)
                {
                    if (mac.Machine.Name == dawControlMachine.MachineState.SoloMachine.TargetMachineName)
                        cbMachineList.SelectedItem = mac;
                }
            }
            else if (type == DAWControlType.Solo2)
            {
                foreach (MachineVM mac in cbMachineList.Items)
                {
                    if (mac.Machine.Name == dawControlMachine.MachineState.SoloMachine2.TargetMachineName)
                        cbMachineList.SelectedItem = mac;
                }
            }
            else if (type == DAWControlType.Solo3)
            {
                foreach (MachineVM mac in cbMachineList.Items)
                {
                    if (mac.Machine.Name == dawControlMachine.MachineState.SoloMachine3.TargetMachineName)
                        cbMachineList.SelectedItem = mac;
                }
            }
            else if (type == DAWControlType.Solo4)
            {
                foreach (MachineVM mac in cbMachineList.Items)
                {
                    if (mac.Machine.Name == dawControlMachine.MachineState.SoloMachine4.TargetMachineName)
                        cbMachineList.SelectedItem = mac;
                }
            }
            else if (type == DAWControlType.Mute)
            {
                foreach (MachineVM mac in cbMachineList.Items)
                {
                    if (mac.Machine.Name == dawControlMachine.MachineState.MuteMachine.TargetMachineName)
                        cbMachineList.SelectedItem = mac;
                }
            }
            else if (type == DAWControlType.Mute2)
            {
                foreach (MachineVM mac in cbMachineList.Items)
                {
                    if (mac.Machine.Name == dawControlMachine.MachineState.MuteMachine2.TargetMachineName)
                        cbMachineList.SelectedItem = mac;
                }
            }
            else if (type == DAWControlType.Mute3)
            {
                foreach (MachineVM mac in cbMachineList.Items)
                {
                    if (mac.Machine.Name == dawControlMachine.MachineState.MuteMachine3.TargetMachineName)
                        cbMachineList.SelectedItem = mac;
                }
            }
            else if (type == DAWControlType.Mute4)
            {
                foreach (MachineVM mac in cbMachineList.Items)
                {
                    if (mac.Machine.Name == dawControlMachine.MachineState.MuteMachine4.TargetMachineName)
                        cbMachineList.SelectedItem = mac;
                }
            }
            else if (type == DAWControlType.Jump1)
            {
                tbJumpTarget.Text = "" + dawControlMachine.MachineState.Jump1.TargetPos;
                cbTick.SelectedIndex = dawControlMachine.MachineState.Jump1.TriggerTick - 1;
            }
            else if (type == DAWControlType.Jump2)
            {
                tbJumpTarget.Text = "" + dawControlMachine.MachineState.Jump2.TargetPos;
                cbTick.SelectedIndex = dawControlMachine.MachineState.Jump2.TriggerTick - 1;
            }
            else if (type == DAWControlType.Jump3)
            {
                tbJumpTarget.Text = "" + dawControlMachine.MachineState.Jump3.TargetPos;
                cbTick.SelectedIndex = dawControlMachine.MachineState.Jump3.TriggerTick - 1;
            }
            else if (type == DAWControlType.Jump4)
            {
                tbJumpTarget.Text = "" + dawControlMachine.MachineState.Jump4.TargetPos;
                cbTick.SelectedIndex = dawControlMachine.MachineState.Jump4.TriggerTick - 1;
            }

            UpdateTypeData();
        }

        private void Graph_MachineRemoved(IMachine obj)
        {
            if (Machines.Remove(Machines.First(m => m.Machine == obj)))
            {
                if (type == DAWControlType.Solo)
                {
                    if (obj.Name == dawControlMachine.MachineState.SoloMachine.TargetMachineName)
                    {
                        cbMachineList.SelectedIndex = -1;
                        dawControlMachine.MachineState.SoloMachine.TargetMachineName = "";
                    }
                }
                else if (type == DAWControlType.Solo2)
                {
                    if (obj.Name == dawControlMachine.MachineState.SoloMachine2.TargetMachineName)
                    {
                        cbMachineList.SelectedIndex = -1;
                        dawControlMachine.MachineState.SoloMachine2.TargetMachineName = "";
                    }
                }
                else if (type == DAWControlType.Solo3)
                {
                    if (obj.Name == dawControlMachine.MachineState.SoloMachine3.TargetMachineName)
                    {
                        cbMachineList.SelectedIndex = -1;
                        dawControlMachine.MachineState.SoloMachine3.TargetMachineName = "";
                    }
                }
                else if (type == DAWControlType.Solo4)
                {
                    if (obj.Name == dawControlMachine.MachineState.SoloMachine4.TargetMachineName)
                    {
                        cbMachineList.SelectedIndex = -1;
                        dawControlMachine.MachineState.SoloMachine4.TargetMachineName = "";
                    }
                }
                else if (type == DAWControlType.Mute)
                {
                    if (obj.Name == dawControlMachine.MachineState.MuteMachine.TargetMachineName)
                    {
                        cbMachineList.SelectedIndex = -1;
                        dawControlMachine.MachineState.MuteMachine.TargetMachineName = "";
                    }
                }
                else if (type == DAWControlType.Mute2)
                {
                    if (obj.Name == dawControlMachine.MachineState.MuteMachine2.TargetMachineName)
                    {
                        cbMachineList.SelectedIndex = -1;
                        dawControlMachine.MachineState.MuteMachine2.TargetMachineName = "";
                    }
                }
                else if (type == DAWControlType.Mute3)
                {
                    if (obj.Name == dawControlMachine.MachineState.MuteMachine3.TargetMachineName)
                    {
                        cbMachineList.SelectedIndex = -1;
                        dawControlMachine.MachineState.MuteMachine3.TargetMachineName = "";
                    }
                }
                else if (type == DAWControlType.Mute4)
                {
                    if (obj.Name == dawControlMachine.MachineState.MuteMachine4.TargetMachineName)
                    {
                        cbMachineList.SelectedIndex = -1;
                        dawControlMachine.MachineState.MuteMachine4.TargetMachineName = "";
                    }
                }
            }
        }

        private void Graph_MachineAdded(IMachine obj)
        {
            Machines.Add(new MachineVM(obj));
        }

        public void UpdateTypeData()
        {
            switch (type)
            {
                case DAWControlType.Play:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.Play.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.Play.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.Play.MidiCC;
                    break;
                case DAWControlType.Stop:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.Stop.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.Stop.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.Stop.MidiCC;
                    break;
                case DAWControlType.Record:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.Rec.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.Rec.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.Rec.MidiCC;
                    break;
                case DAWControlType.Forward:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.Forward.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.Forward.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.Forward.MidiCC;
                    break;
                case DAWControlType.Rewind:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.Rewind.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.Rewind.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.Rewind.MidiCC;
                    break;
                case DAWControlType.LoopStart:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.LoopStart.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.LoopStart.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.LoopStart.MidiCC;
                    break;
                case DAWControlType.Solo:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.SoloMachine.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.SoloMachine.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.SoloMachine.MidiCC;
                    break;
                case DAWControlType.Solo2:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.SoloMachine2.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.SoloMachine2.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.SoloMachine2.MidiCC;
                    break;
                case DAWControlType.Solo3:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.SoloMachine3.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.SoloMachine3.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.SoloMachine3.MidiCC;
                    break;
                case DAWControlType.Solo4:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.SoloMachine4.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.SoloMachine4.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.SoloMachine4.MidiCC;
                    break;
                case DAWControlType.Mute:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.MuteMachine.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.MuteMachine.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.MuteMachine.MidiCC;
                    break;
                case DAWControlType.Mute2:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.MuteMachine2.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.MuteMachine2.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.MuteMachine2.MidiCC;
                    break;
                case DAWControlType.Mute3:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.MuteMachine3.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.MuteMachine3.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.MuteMachine3.MidiCC;
                    break;
                case DAWControlType.Mute4:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.MuteMachine4.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.MuteMachine4.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.MuteMachine4.MidiCC;
                    break;
                case DAWControlType.LoopToggle:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.LoopToggle.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.LoopToggle.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.LoopToggle.MidiCC;
                    break;
                case DAWControlType.IncreaseSpeed:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.IncreaseSpeed.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.IncreaseSpeed.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.IncreaseSpeed.MidiCC;
                    break;
                case DAWControlType.DecreaseSpeed:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.DecreaseSpeed.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.DecreaseSpeed.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.DecreaseSpeed.MidiCC;
                    break;
                case DAWControlType.Jump1:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.Jump1.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.Jump1.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.Jump1.MidiCC;
                    break;
                case DAWControlType.Jump2:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.Jump2.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.Jump2.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.Jump2.MidiCC;
                    break;
                case DAWControlType.Jump3:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.Jump3.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.Jump3.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.Jump3.MidiCC;
                    break;
                case DAWControlType.Jump4:
                    cbChannel.SelectedIndex = dawControlMachine.MachineState.Jump4.MidiChannel;
                    tbNoteValue.Text = "" + dawControlMachine.MachineState.Jump4.MidiNoteValue;
                    cbCCValue.SelectedIndex = dawControlMachine.MachineState.Jump4.MidiCC;
                    break;
            }
        }

        private void UpdateSelectedMachineData()
        {
            switch (type)
            {
                case DAWControlType.Solo:
                    if (cbMachineList.SelectedItem != null)
                        dawControlMachine.MachineState.SoloMachine.TargetMachineName = cbMachineList.SelectedItem.ToString();                        
                    break;
                case DAWControlType.Solo2:
                    if (cbMachineList.SelectedItem != null)
                        dawControlMachine.MachineState.SoloMachine2.TargetMachineName = cbMachineList.SelectedItem.ToString();
                    break;
                case DAWControlType.Solo3:
                    if (cbMachineList.SelectedItem != null)
                        dawControlMachine.MachineState.SoloMachine3.TargetMachineName = cbMachineList.SelectedItem.ToString();
                    break;
                case DAWControlType.Solo4:
                    if (cbMachineList.SelectedItem != null)
                        dawControlMachine.MachineState.SoloMachine4.TargetMachineName = cbMachineList.SelectedItem.ToString();
                    break;
                case DAWControlType.Mute:
                    if (cbMachineList.SelectedItem != null)
                        dawControlMachine.MachineState.MuteMachine.TargetMachineName = cbMachineList.SelectedItem.ToString();
                    break;
                case DAWControlType.Mute2:
                    if (cbMachineList.SelectedItem != null)
                        dawControlMachine.MachineState.MuteMachine2.TargetMachineName = cbMachineList.SelectedItem.ToString();
                    break;
                case DAWControlType.Mute3:
                    if (cbMachineList.SelectedItem != null)
                        dawControlMachine.MachineState.MuteMachine3.TargetMachineName = cbMachineList.SelectedItem.ToString();
                    break;
                case DAWControlType.Mute4:
                    if (cbMachineList.SelectedItem != null)
                        dawControlMachine.MachineState.MuteMachine4.TargetMachineName = cbMachineList.SelectedItem.ToString();
                    break;
            }
        }

        private void UpdateChannelData()
        {
            switch (type)
            {
                case DAWControlType.Play:

                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.Play.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.Play.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Stop:

                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.Stop.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.Stop.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Record:

                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.Rec.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.Rec.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Forward:

                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.Forward.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.Forward.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Rewind:

                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.Rewind.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.Rewind.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.LoopStart:

                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.LoopStart.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.LoopStart.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Solo:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.SoloMachine.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.SoloMachine.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Solo2:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.SoloMachine2.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.SoloMachine2.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Solo3:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.SoloMachine3.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.SoloMachine3.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Solo4:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.SoloMachine4.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.SoloMachine4.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Mute:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.MuteMachine.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.MuteMachine.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Mute2:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.MuteMachine2.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.MuteMachine2.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Mute3:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.MuteMachine3.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.MuteMachine3.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.Mute4:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.MuteMachine4.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.MuteMachine4.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.LoopToggle:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.LoopToggle.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.LoopToggle.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.IncreaseSpeed:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.IncreaseSpeed.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.IncreaseSpeed.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
                case DAWControlType.DecreaseSpeed:
                    if ((string)this.cbChannel.SelectedItem == "Disabled")
                        dawControlMachine.MachineState.DecreaseSpeed.MidiChannel = 0;
                    else
                        dawControlMachine.MachineState.DecreaseSpeed.MidiChannel = Int32.Parse((string)this.cbChannel.SelectedItem);
                    break;
            }
        }

        private void UpdateNoteValueData()
        {
            switch (type)
            {
                case DAWControlType.Play:
                    dawControlMachine.MachineState.Play.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Stop:
                    dawControlMachine.MachineState.Stop.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Record:
                    dawControlMachine.MachineState.Rec.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Forward:
                    dawControlMachine.MachineState.Forward.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Rewind:
                    dawControlMachine.MachineState.Rewind.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.LoopStart:
                    dawControlMachine.MachineState.LoopStart.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Solo:
                    dawControlMachine.MachineState.SoloMachine.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Solo2:
                    dawControlMachine.MachineState.SoloMachine2.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Solo3:
                    dawControlMachine.MachineState.SoloMachine3.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Solo4:
                    dawControlMachine.MachineState.SoloMachine4.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Mute:
                    dawControlMachine.MachineState.MuteMachine.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Mute2:
                    dawControlMachine.MachineState.MuteMachine2.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Mute3:
                    dawControlMachine.MachineState.MuteMachine3.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.Mute4:
                    dawControlMachine.MachineState.MuteMachine4.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.LoopToggle:
                    dawControlMachine.MachineState.LoopToggle.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.IncreaseSpeed:
                    dawControlMachine.MachineState.IncreaseSpeed.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                case DAWControlType.DecreaseSpeed:
                    dawControlMachine.MachineState.DecreaseSpeed.MidiNoteValue = ParseNumber((string)tbNoteValue.Text);
                    break;
                    
            }
        }

        private int ParseNumber(string str)
        {
            int ret = 0;
            try
            {
                ret = Int32.Parse(str);
            }
            catch
            {
            }

            return ret;
        }
        
        private void UpdateCCData()
        {
            switch (type)
            {
                case DAWControlType.Play:
                    dawControlMachine.MachineState.Play.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Stop:
                    dawControlMachine.MachineState.Stop.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Record:
                    dawControlMachine.MachineState.Rec.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Forward:
                    dawControlMachine.MachineState.Forward.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Rewind:
                    dawControlMachine.MachineState.Rewind.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.LoopStart:
                    dawControlMachine.MachineState.LoopStart.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Solo:
                    dawControlMachine.MachineState.SoloMachine.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Solo2:
                    dawControlMachine.MachineState.SoloMachine2.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Solo3:
                    dawControlMachine.MachineState.SoloMachine3.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Solo4:
                    dawControlMachine.MachineState.SoloMachine4.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Mute:
                    dawControlMachine.MachineState.MuteMachine.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Mute2:
                    dawControlMachine.MachineState.MuteMachine2.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Mute3:
                    dawControlMachine.MachineState.MuteMachine3.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.Mute4:
                    dawControlMachine.MachineState.MuteMachine4.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.LoopToggle:
                    dawControlMachine.MachineState.LoopToggle.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.IncreaseSpeed:
                    dawControlMachine.MachineState.IncreaseSpeed.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;
                case DAWControlType.DecreaseSpeed:
                    dawControlMachine.MachineState.DecreaseSpeed.MidiCC = Int32.Parse((string)this.cbCCValue.SelectedItem);
                    break;

            }
        }
        private void TbJumpTarget_SelectionChanged(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case DAWControlType.Jump1:
                    dawControlMachine.MachineState.Jump1.TargetPos = ParseNumber((string)tbJumpTarget.Text);
                    break;
                case DAWControlType.Jump2:
                    dawControlMachine.MachineState.Jump2.TargetPos = ParseNumber((string)tbJumpTarget.Text);
                    break;
                case DAWControlType.Jump3:
                    dawControlMachine.MachineState.Jump3.TargetPos = ParseNumber((string)tbJumpTarget.Text);
                    break;
                case DAWControlType.Jump4:
                    dawControlMachine.MachineState.Jump4.TargetPos = ParseNumber((string)tbJumpTarget.Text);
                    break;
            }
        }

        private void UpdateJumpTick(DAWControlType type)
        {
            switch (type)
            {
                case DAWControlType.Jump1:
                    dawControlMachine.MachineState.Jump1.TriggerTick = (int)this.cbTick.SelectedItem;
                    break;
                case DAWControlType.Jump2:
                    dawControlMachine.MachineState.Jump2.TriggerTick = (int)this.cbTick.SelectedItem;
                    break;
                case DAWControlType.Jump3:
                    dawControlMachine.MachineState.Jump3.TriggerTick = (int)this.cbTick.SelectedItem;
                    break;
                case DAWControlType.Jump4:
                    dawControlMachine.MachineState.Jump4.TriggerTick = (int)this.cbTick.SelectedItem;
                    break;
            }
        }

        internal void RemoveEvents()
        {
            dawControlMachine.getHost().Machine.Graph.MachineAdded -= Graph_MachineAdded;
            dawControlMachine.getHost().Machine.Graph.MachineRemoved -= Graph_MachineRemoved;
        }
    }
}
