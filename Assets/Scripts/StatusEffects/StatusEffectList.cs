using System;
using System.Linq;
using Entities;
using UnityEngine.Events;

namespace StatusEffects
{
    public class StatusEffectList
    {
        private readonly Entity _entity;
        private StatusEffect[] _effects;

        private int _capacity = 4;
        
        public int Count;

        private Action<string> _print;
        
        public StatusEffect this[int index] => _effects[index];
        
        public StatusEffectList(Entity entity, Action<string> print)
        {
            _entity = entity;
            
            _effects = new StatusEffect[_capacity];
            
            _print = print;
        }
        
        public void Update()
        {
            for (int i = 0; i < Count; i++)
            {
                var effect = _effects[i];
                effect.Update();
                
                if (effect.Expired)
                    Remove(effect);
            }
        }

        public int GetCount(StatusEffect sampleEffect)
        {
            var type = sampleEffect.GetType();
            
            int count = 0;
            for (int i = 0; i < Count; i++)
            {
                if (_effects[i] == null)
                    break;
                
                if (_effects[i].GetType() == type)
                    count++;
            }

            return count;
        }
        
        private bool HasEffect(StatusEffect sampleEffect)
        {
            var type = sampleEffect.GetType();

            for (int i = 0; i < Count; i++)
            {
                if (_effects[i] == null)
                    return false;
                
                if (_effects[i].GetType() == type)
                    return true;
            }

            return false;
        }
        
        public void Add(StatusEffect effect)
        {
            if (effect.RefreshOnApplication)
                Refresh(effect);

            if (!effect.Stackable && HasEffect(effect))
                return;
            
            if (Count == _capacity)
                Extend();
            
            _effects[Count++] = effect;
            effect.Apply(_entity);
        }
        
        public void Remove(StatusEffect effect)
        {
            int i = 0;
            for (; i < Count; i++)
            {
                if (_effects[i] == effect)
                    break;
            }

            _effects[i].Remove(_entity);
            _effects[i] = null;
            
            Rebuild();
            Count--;
        }

        public void RemoveAll(StatusEffect sampleEffect, int max = int.MaxValue)
        {
            var type = sampleEffect.GetType();
            
            int count = 0;
            for (int i = Count - 1; i >= 0; i--)
            {
                if (count >= max)
                    break;
                
                if (_effects[i].GetType() != type)
                    continue;
                
                count++;
                _effects[i].Remove(_entity);
                _effects[i] = null;
                Count--;
            }
            
            Rebuild();
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                _effects[i].Remove(_entity);
                _effects[i] = null;
            }
            
            Count = 0;
        }

        private void Extend()
        {
            _capacity *= 2;
            
            var newArray = new StatusEffect[_capacity];
            for (int i = 0; i < _effects.Length; i++)
            {
                newArray[i] = _effects[i];
            }
            
            _effects = newArray;
        }

        private void Rebuild()
        {
            int nextIndex = 1;
            for (int i = 0; i < _capacity - 1; i++)
            {
                if (_effects[i] != null)
                {
                    nextIndex++;
                    continue;
                }
                
                for (int j = nextIndex; j < _capacity; j++)
                {
                    if (_effects[j] == null)
                        continue;

                    _effects[i] = _effects[j];
                    _effects[j] = null;

                    nextIndex = j + 1;
                    
                    break;
                }
            }
        }

        private void Refresh(StatusEffect sampleEffect)
        {
            var effectType = sampleEffect.GetType();
            
            foreach (var effect in _effects)
            {
                if (effect == null || effect.GetType() != effectType)
                    continue;
                
                effect.RefreshDuration();
            }
        }
    }
}