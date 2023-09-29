using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components {
    public readonly struct GeneratorRequirementComponent : IComponent{
        private readonly Dictionary<string, object?> _generatorData;
        public object? this[string key] {
            get { return _generatorData.GetValueOrDefault(key,null); }
            set { _generatorData[key]=value; }
        }
        public GeneratorRequirementComponent() {
            _generatorData = new();
        }

        public IComponent Clone() {
            return new GeneratorRequirementComponent();
        }
    }
}
