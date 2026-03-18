using System;
using System.Linq;
using Entities;
using UnityEngine.Events;

namespace StatusEffects
{
    public class StatusEffectList
    {
        private readonly Enemy _enemy;
        private StatusEffect[] _effects;

        private int _capacity = 4;
        
        public int Count;
        
        public StatusEffect this[int index] => _effects[index];
        
        public StatusEffectList(Enemy enemy)
        {
            _enemy = enemy;
            
            _effects = new StatusEffect[_capacity];
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

        public int GetCount<T>() where T : StatusEffect
        {
            var type = typeof(T);
            
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

        public bool HasEffect<T>() where T : StatusEffect
        {
            for (int i = 0; i < Count; i++)
            {
                if (_effects[i] == null)
                    return false;
                
                if (_effects[i].GetType() == typeof(T))
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
            effect.Apply(_enemy);
        }
        
        public void Remove(StatusEffect effect)
        {
            int i = 0;
            for (; i < Count; i++)
            {
                if (_effects[i] == effect)
                    break;
            }

            _effects[i].Remove(_enemy);
            _effects[i] = null;
            
            Rebuild();
            Count--;
        }

        public void RemoveAll<T>() where T : StatusEffect
        {
            var type = typeof(T);
            
            for (int i = Count - 1; i >= 0; i--)
            {
                if (_effects[i].GetType() != type)
                    continue;
                
                _effects[i].Remove(_enemy);
                _effects[i] = null;
                Count--;
            }
            
            Rebuild();
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                _effects[i].Remove(_enemy);
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