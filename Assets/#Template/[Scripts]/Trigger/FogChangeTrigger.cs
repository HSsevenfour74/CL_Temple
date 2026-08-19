using System;
using DG.Tweening;
using UnityEngine;

namespace Triggers
{
	// Token: 0x020001ED RID: 493
	public class FogChangeTrigger : Trigger
	{
		// Token: 0x0600160A RID: 5642 RVA: 0x0000263E File Offset: 0x0000083E
		protected override void OnTriggerEnter(Collider col)
		{
		}

		// Token: 0x04000E02 RID: 3586
		public float startDist;

		// Token: 0x04000E03 RID: 3587
		public float endDist;

		// Token: 0x04000E04 RID: 3588
		public float duration;

		// Token: 0x04000E05 RID: 3589
		public Ease ease;

		// Token: 0x04000E06 RID: 3590
		public bool AnimateFogColor;

		// Token: 0x04000E07 RID: 3591
		public Color FogColor;
	}
}
