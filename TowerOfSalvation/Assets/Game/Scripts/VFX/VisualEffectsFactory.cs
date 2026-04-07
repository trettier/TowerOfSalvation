using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TowerOfSalvation.Effects
{
    public class VisualEffectsFactory : Singleton<VisualEffectsFactory>
    {
        [Header("Настройки пула")]
        [SerializeField] private bool _usePooling = true;
        [SerializeField] private int _defaultPoolSize = 20;
        [SerializeField] private Transform _poolContainer;

        [Header("Префабы эффектов")]
        [SerializeField] private List<EffectPrefab> _effectPrefabs = new List<EffectPrefab>();

        [Header("Настройки")]
        [SerializeField] private bool _autoReturnToPool = true;
        [SerializeField] private float _defaultEffectDuration = 2f;

        private Dictionary<EffectType, GameObject> _prefabDict;
        private Dictionary<EffectType, Queue<GameObject>> _pools;
        private Dictionary<EffectType, List<GameObject>> _activeEffects;

        [Serializable]
        public class EffectPrefab
        {
            public EffectType type;
            public GameObject prefab;
            public int poolSize = 10;
            public float defaultDuration = 2f;
        }

        public enum EffectType
        {
            None,
            Hit
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeFactory();
        }

        private void InitializeFactory()
        {
            _prefabDict = new Dictionary<EffectType, GameObject>();
            _pools = new Dictionary<EffectType, Queue<GameObject>>();
            _activeEffects = new Dictionary<EffectType, List<GameObject>>();

            if (_poolContainer == null)
            {
                GameObject container = new GameObject("EffectPool");
                container.transform.parent = transform;
                _poolContainer = container.transform;
            }

            foreach (var effectPrefab in _effectPrefabs)
            {
                _prefabDict[effectPrefab.type] = effectPrefab.prefab;

                if (_usePooling)
                {
                    CreatePool(effectPrefab.type, effectPrefab.poolSize);
                }
            }
        }

        private void CreatePool(EffectType type, int size)
        {
            if (!_prefabDict.ContainsKey(type) || _prefabDict[type] == null)
            {
                Debug.LogError($"No prefab for effect type: {type}");
                return;
            }

            var pool = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject effect = CreateNewEffect(type);
                effect.SetActive(false);
                effect.transform.SetParent(_poolContainer);
                pool.Enqueue(effect);
            }

            _pools[type] = pool;
            _activeEffects[type] = new List<GameObject>();
        }

        private GameObject CreateNewEffect(EffectType type)
        {
            if (!_prefabDict.ContainsKey(type) || _prefabDict[type] == null)
            {
                Debug.LogError($"No prefab for effect type: {type}");
                return null;
            }

            GameObject effect = Instantiate(_prefabDict[type]);
            effect.name = $"{type}_{Guid.NewGuid().ToString().Substring(0, 5)}";

            return effect;
        }

        /// <summary>
        /// Создать эффект в позиции
        /// </summary>
        public GameObject SpawnEffect(EffectType type, Vector3 position, Quaternion rotation = default, float duration = -1)
        {
            if (type == EffectType.None) return null;

            GameObject effect = GetEffectFromPool(type);

            if (effect == null) return null;

            effect.transform.position = position;
            effect.transform.rotation = rotation;
            effect.SetActive(true);

            if (_activeEffects.ContainsKey(type))
            {
                _activeEffects[type].Add(effect);
            }

            float effectDuration = duration >= 0 ? duration : GetDefaultDuration(type);

            if (_autoReturnToPool && effectDuration > 0)
            {
                StartCoroutine(ReturnToPoolAfterDelay(effect, type, effectDuration));
            }

            return effect;
        }

        private GameObject GetEffectFromPool(EffectType type)
        {
            if (!_usePooling)
            {
                return CreateNewEffect(type);
            }

            if (!_pools.ContainsKey(type))
            {
                CreatePool(type, _defaultPoolSize);
            }

            var pool = _pools[type];

            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            else
            {
                return CreateNewEffect(type);
            }
        }

        private IEnumerator ReturnToPoolAfterDelay(GameObject effect, EffectType type, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToPool(effect, type);
        }

        /// <summary>
        /// Вернуть эффект в пул
        /// </summary>
        public void ReturnToPool(GameObject effect, EffectType type)
        {
            if (effect == null) return;

            effect.SetActive(false);
            effect.transform.SetParent(_poolContainer);

            if (_usePooling)
            {
                if (!_pools.ContainsKey(type))
                {
                    _pools[type] = new Queue<GameObject>();
                }

                _pools[type].Enqueue(effect);
            }
            else
            {
                Destroy(effect);
            }

            if (_activeEffects.ContainsKey(type) && _activeEffects[type].Contains(effect))
            {
                _activeEffects[type].Remove(effect);
            }
        }

        /// <summary>
        /// Вернуть все активные эффекты в пул
        /// </summary>
        public void ReturnAllToPool()
        {
            foreach (var kvp in _activeEffects)
            {
                var effectsCopy = new List<GameObject>(kvp.Value);
                foreach (var effect in effectsCopy)
                {
                    ReturnToPool(effect, kvp.Key);
                }
            }
        }

        /// <summary>
        /// Создать эффект в позиции с привязкой к родителю
        /// </summary>
        public GameObject SpawnEffectAttached(EffectType type, Transform parent, Vector3 localPosition = default, Quaternion localRotation = default)
        {
            GameObject effect = SpawnEffect(type, parent.position, localRotation);

            if (effect != null)
            {
                effect.transform.SetParent(parent);
                effect.transform.localPosition = localPosition;
            }

            return effect;
        }

        /// <summary>
        /// Создать эффект удара
        /// </summary>
        public void SpawnHitEffect(Vector3 position)
        {
            SpawnEffect(EffectType.Hit, position);
        }

        private float GetDefaultDuration(EffectType type)
        {
            foreach (var prefab in _effectPrefabs)
            {
                if (prefab.type == type)
                {
                    return prefab.defaultDuration;
                }
            }

            return _defaultEffectDuration;
        }

        /// <summary>
        /// Проверить, есть ли активные эффекты данного типа
        /// </summary>
        public bool HasActiveEffects(EffectType type)
        {
            return _activeEffects.ContainsKey(type) && _activeEffects[type].Count > 0;
        }

        /// <summary>
        /// Получить количество активных эффектов
        /// </summary>
        public int GetActiveCount(EffectType type)
        {
            return _activeEffects.ContainsKey(type) ? _activeEffects[type].Count : 0;
        }

        /// <summary>
        /// Предзагрузить пулы для всех эффектов
        /// </summary>
        public void PreloadPools()
        {
            foreach (var effectPrefab in _effectPrefabs)
            {
                if (!_pools.ContainsKey(effectPrefab.type))
                {
                    CreatePool(effectPrefab.type, effectPrefab.poolSize);
                }
            }
        }

        /// <summary>
        /// Очистить пулы
        /// </summary>
        public void ClearPools()
        {
            ReturnAllToPool();

            foreach (var pool in _pools.Values)
            {
                while (pool.Count > 0)
                {
                    var effect = pool.Dequeue();
                    if (effect != null)
                    {
                        Destroy(effect);
                    }
                }
            }

            _pools.Clear();
            _activeEffects.Clear();
        }

        private void OnDestroy()
        {
            ClearPools();
        }
    }
}