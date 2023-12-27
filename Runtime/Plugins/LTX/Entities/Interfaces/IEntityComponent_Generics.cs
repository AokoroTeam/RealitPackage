using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTX.Entities
{
    public interface IEntityComponent<T> : IEntityComponent where T : Entity
    {
        T Manager { get; set; }
        void Initiate(T manager);
    }
}
