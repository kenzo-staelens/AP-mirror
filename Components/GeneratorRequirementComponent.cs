namespace Components {
    public readonly struct GeneratorRequirementComponent : IComponent {
        private readonly Dictionary<string, object?> _generatorData;
        public object? this[string key] {
            get { return _generatorData.GetValueOrDefault(key, null); }
            set { _generatorData[key] = value; }
        }
        private GeneratorRequirementComponent(Dictionary<string, object?> dict) {
            //let op voor reference types in value
            _generatorData = dict.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
        public GeneratorRequirementComponent() {
            _generatorData = new();
        }

        public IComponent Clone() {
            return new GeneratorRequirementComponent(_generatorData);
        }
    }
}
