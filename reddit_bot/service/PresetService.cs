using reddit_bor.domain.pool;
using reddit_bor.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reddit_bor.service
{
    public class PresetService
    {
        private readonly PresetRepository _presetRepository;

        public PresetService()
        {
            _presetRepository = new PresetRepository();
        }

        public Preset SavePreset(Preset preset)
        {
            if (_presetRepository.FindByName(preset.Name) != null)
            {
                return null;
            }

            return _presetRepository.Save(preset);
        }

        public List<Preset> FindAllPresets()
        {
            return _presetRepository.FindAll();
        }
    }
}
