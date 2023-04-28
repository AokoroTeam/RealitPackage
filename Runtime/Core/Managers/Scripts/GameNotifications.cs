using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aokoro;
using Aokoro.Sequencing;
using NaughtyAttributes;
using Michsky.MUIP;

namespace Realit.Core.Managers
{
    public class GameNotifications : Singleton<GameNotifications>
    {
        [System.Serializable]
        private class Notification
        {
            [SerializeField]
            float waitTime;
            [SerializeField]
            float duration;

            [SerializeField]
            float currentTime;

            [SerializeField]
            string title;
            [SerializeField]
            string content;

            public Notification(float time, float duration, string title, string content)
            {
                this.waitTime = time;
                this.duration = time;
                this.title = title;
                this.content = content;

                currentTime = 0;
            }

            public bool Update(float deltaTime)
            {
                //Debug.Log($"Notif : {currentTime} to {currentTime + deltaTime}");
                currentTime += deltaTime;

                if(currentTime >= waitTime)
                {
                    //Debug.Log($"Nouvelle notif : {title}");
                    Instance.TriggerNotification(title, content, duration);
                    return true;
                }

                return false;
            }
        }

        [SerializeField]
        GameObject topRight;
        CanvasGroup canvasGroup;

        [SerializeField]
        private List<Notification> notifications = new List<Notification>();
        private List<Notification> corbeille = new List<Notification>();


        public ChanneledProperty<bool> canUpdate = new ChanneledProperty<bool>(true);

        [SerializeField, ShowNativeProperty]
        bool CanUpdate => canUpdate.Value;

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void AddNotificationToQueue(string title, string content, float duration, float time)
        {
            /*
            Notification notification = new Notification(time, duration, title, content);
            notifications.Add(notification);
            */
        }

        private void Update()
        {
            if (canUpdate)
            {
                corbeille.Clear();

                foreach (var notification in notifications)
                {
                    if (notification.Update(Time.deltaTime))
                        corbeille.Add(notification);
                }

                foreach (var remove in corbeille)
                    notifications.Remove(remove);

                canvasGroup.alpha = 1;
            }
            else
            {
                canvasGroup.alpha = 0;
            }
        }

        public void Clear()
        {
            notifications.Clear();
            corbeille.Clear();
        }

        private void TriggerNotification(string title, string content, float duration)
        {
            NotificationManager notification = Instantiate(topRight, transform).GetComponent<NotificationManager>();

            notification.title = title;
            notification.description = content;
            notification.timer = duration;
            notification.UpdateUI();
            notification.OpenNotification();
        }

        protected override void OnExistingInstanceFound(GameNotifications existingInstance)
        {
            Destroy(gameObject);
        }


        
    }
}