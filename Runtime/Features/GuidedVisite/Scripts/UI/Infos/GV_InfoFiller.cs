using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Core.Features.GuidedVisite
{
    public class GV_InfoFiller : MonoBehaviour
    {
        [SerializeField]
        GameObject prefab;
        [SerializeField]
        Transform parent;

        List<GV_InfoField> fields = new List<GV_InfoField>();

        public void SetupInfos(List<Info> infos, Dictionary<InfoType, InfoRepresentation> reps)
        {
            ClearFields();

            foreach (var info in infos)
            {
                if (reps.TryGetValue(info.type, out var rep))
                {
                    GV_InfoField field = Instantiate(prefab, parent).GetComponent<GV_InfoField>();
                    field.SetData(rep.sprite, rep.label, info.data);
                    fields.Add(field);
                }
            }
        }

        private void ClearFields()
        {
            foreach (var field in fields)
                Destroy(field.gameObject);

            fields.Clear();
        }

        public void Setup()
        {
            var existing = parent.GetComponentsInChildren<GV_InfoField>();
            for (int i = 0; i < existing.Length; i++)
                Destroy(existing[i].gameObject);
        }

    }
}
