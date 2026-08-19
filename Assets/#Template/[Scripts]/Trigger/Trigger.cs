using System;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
	// Token: 0x020001EE RID: 494
	public class Trigger : MonoBehaviour
	{
		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600160C RID: 5644 RVA: 0x00057FE7 File Offset: 0x000561E7
		// (set) Token: 0x0600160D RID: 5645 RVA: 0x00057FEF File Offset: 0x000561EF
		public bool Enabled { get; set; }

		// Token: 0x0600160E RID: 5646 RVA: 0x0000263E File Offset: 0x0000083E
		private void Start()
		{
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x00057FF8 File Offset: 0x000561F8
		protected virtual void OnTriggerEnter(Collider col)
		{
			if (this.TrigerableContainer != null)
			{
				ITrigerable trigerable = (ITrigerable)this.TrigerableContainer.GetComponent(typeof(ITrigerable));
				if (trigerable != null)
				{
					trigerable.TriggerMe(this);
				}
				Component[] componentsInChildren = this.TrigerableContainer.GetComponentsInChildren(typeof(ITrigerable));
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					((ITrigerable)componentsInChildren[i]).TriggerMe(this);
				}
			}
			this.Enabled = false;
			this.Event.Invoke();
		}

		// Token: 0x04000E08 RID: 3592
		public GameObject TrigerableContainer;

		// Token: 0x04000E09 RID: 3593
		public UnityEvent Event = new UnityEvent();
	}
}
