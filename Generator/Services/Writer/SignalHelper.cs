using System.Linq;
using GirLoader.Output.Model;

namespace Generator.Services.Writer
{
    internal class SignalHelper
    {
        private readonly Signal _signal;

        public bool NeedsArgs => _signal.ParameterList.Any();
        public string EventName => $"On{_signal.Name}";
        public string EventDescriptor => $"{_signal.Name}Signal";
        public string ArgsType => _signal.Name + "SignalArgs";

        public SignalHelper(Signal signal)
        {
            _signal = signal;
        }
    }
}
