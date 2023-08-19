using LTX.ChanneledProperties;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        public event Action<Entity> OnEntityInitiated;

        IUpdateEntityComponent[] Ucomponents;
        ILateUpdateEntityComponent[] LUcomponents;
        IFixedUpdateEntityComponent[] FUcomponents;

        Dictionary<string, IEntityComponent> components;
        Dictionary<Type, IEntityComponent> getEntityComponentCache;

        public PrioritisedProperty<bool> Freezed;

        private bool isReady = false;

        protected virtual void Awake()
        {
            SetupVariables();

            Initiate<Entity>();
        }

        protected void SetupVariables()
        {
            Freezed = new PrioritisedProperty<bool>(false);
            getEntityComponentCache = new Dictionary<Type, IEntityComponent>();
        }

        protected virtual void Initiate<T>() where T : Entity
        {
            var componentsArray = InitializeComponents<T>();
            components = new Dictionary<string, IEntityComponent>(componentsArray.Length);

            List<IUpdateEntityComponent> updatesList = new();
            List<ILateUpdateEntityComponent> lateUpdatesList = new();
            List<IFixedUpdateEntityComponent> fixedUpdatesList = new();

            for (int i = 0; i < componentsArray.Length; i++)
            {
                IEntityComponent component = componentsArray[i];
                if (component is IUpdateEntityComponent u)
                    updatesList.Add(u);
                if (component is IFixedUpdateEntityComponent fu)
                    fixedUpdatesList.Add(fu);
                if (component is ILateUpdateEntityComponent lu)
                    lateUpdatesList.Add(lu);

                components.Add(component.ComponentName, component);
            }

            Ucomponents = updatesList.ToArray();
            FUcomponents = fixedUpdatesList.ToArray();
            LUcomponents = lateUpdatesList.ToArray();

            isReady = true;
            OnEntityInitiated?.Invoke(this);
        }

        private IEntityComponent[] InitializeComponents<T>() where T : Entity
        {
            if(this is not T typedManager)
            {
                Debug.LogError($"{this.GetType().Name} is not of type {typeof(T).Name}");
                return new IEntityComponent[0];
            }

            var global = GetComponentsInChildren<IEntityComponent<T>>();

            for (int i = 0; i < global.Length; i++)
            {
                global[i].Manager = typedManager;
                global[i].Initiate(this as T);
            }

            return global;
        }

        protected virtual void Update()
        {
            if (isReady && !Freezed.Value)
            {
                for (int i = 0; i < Ucomponents.Length; i++)
                {
                    try
                    {
                        Ucomponents[i].OnUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e, Ucomponents[i] as MonoBehaviour);
                    }
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (isReady && !Freezed.Value)
            {
                for (int i = 0; i < FUcomponents.Length; i++)
                {
                    try
                    {
                        FUcomponents[i].OnFixedUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e, FUcomponents[i] as MonoBehaviour);
                    }
                }
            }
        }

        protected virtual void LateUpdate()
        {
            if (isReady && !Freezed.Value)
            {
                for (int i = 0; i < LUcomponents.Length; i++)
                {
                    try
                    {
                        LUcomponents[i].OnLateUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }


        public bool GetLivingComponent<T>(out T component) where T : IEntityComponent
        {
            component = default;

            if(getEntityComponentCache.TryGetValue(typeof(T), out IEntityComponent entityComponent))
            {
                component = (T)entityComponent;
                return true;
            }


            foreach (IEntityComponent c1 in components.Values)
            {
                if (c1 is T c)
                {
                    component = c;

                    getEntityComponentCache.Add(typeof(T), component);
                    return true;
                }
            }

            return false;
        }
        public T GetLivingComponent<T>() where T : IEntityComponent
        {
            if(getEntityComponentCache.TryGetValue(typeof(T), out IEntityComponent entityComponent))
                return (T)entityComponent;

            foreach (IEntityComponent component in components.Values)
            {
                if (component is T c)
                {
                    getEntityComponentCache.Add(typeof(T), component);
                    return c;
                }
            }

            return default;
        }
        public T GetLivingComponent<T>(string name) where T : IEntityComponent
        {
            if (components.TryGetValue(name, out IEntityComponent entityComponent) && entityComponent is T result)
                return result;
            
            return default;
        }

        public bool GetLivingComponent<T>(string name, out T component) where T : IEntityComponent
        {
            if (components.TryGetValue(name, out IEntityComponent entityComponent) && entityComponent is T result)
            {
                component = result;
                return true;
            }

            component = default;
            return false;
        }

        public void Log(string message)
        {
            Debug.Log(string.Concat("[", gameObject.name, "] => ", message));
        }


    }
}
