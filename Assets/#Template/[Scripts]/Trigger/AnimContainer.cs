using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnimContainer : MonoBehaviour
{
	public enum AddType
	{
		Append = 0,
		Join = 1,
		Prepend = 2,
		Insert = 3
	}

	public enum AnimType
	{
		LocalPosition = 0,
		LocalPositionX = 1,
		LocalPositionY = 2,
		LocalPositionZ = 3,
		Position = 4,
		PositionX = 5,
		PositionY = 6,
		PositionZ = 7,
		Scale = 8,
		RotateVector = 9,
		RotateQuaternion = 10,
		FadeImageOrGroup = 11,
		ShakePosition = 12,
		ShakeRotation = 13,
		ShakeScale = 14
	}

	[Serializable]
	public class AnimParameters
	{
		public class AnimParametersComparer : IComparer<AnimParameters>
		{
			public int Compare(AnimParameters x, AnimParameters y)
			{
				if (x.startTime == y.startTime)
				{
					return (x.startTime + x.duration).CompareTo(y.startTime + y.duration);
				}
				return x.startTime.CompareTo(y.startTime);
			}
		}

		public string animName = string.Empty;

		public bool moveUpInEditor;

		public bool moveDownInEditor;

		public float insertPosition;

		public AddType addType;

		public AnimType animType;

		public RotateMode rotateMode;

		public float startTime;

		public float startTimeEnd;

		public float duration = 1f;

		public bool durationIsSpeed;

		public Ease animEase;

		public int loopCount;

		public LoopType loopType;

		public bool overrideStartVal = true;

		public Vector3 startValue;

		public Vector3 endValue;

		public Transform endValueTrans;

		public Vector3 shakeStrength = new Vector3(1f, 1f, 0f);

		public int shakeVibrato = 10;

		public float shakeRandomness = 90f;

		public UnityEvent OnStart;

		public UnityEvent OnEnd;

		public UnityEvent OnStopOrKill;

		public string NameWithDetails()
		{
			return animType.ToString() + ": " + (startTime + insertPosition) + "-" + (startTime + duration + insertPosition);
		}
	}

	public bool DEBUG;

	public bool TEST_START;

	public bool TEST_STOP;

	public UnityEvent OnStart;

	public UnityEvent OnEnd;

	public UnityEvent OnStop;

	public UnityEvent OnRewind;

	public bool useIndependentTime = true;

	public bool enableDisableObject;

	public bool onStopPlayBackwards;

	public bool starOnEnable;

	public int loopCount;

	public LoopType loopType;

	public float loopDelayFrom;

	public float loopDelayTo;

	public bool onLoopStopRewind;

	public List<AnimParameters> anims;

	private Sequence animSequence;

	private List<Tweener> tweens = new List<Tweener>();

	private Transform trans;

	private RectTransform rectTrans;

	public Transform Trans
	{
		get
		{
			return trans;
		}
	}

	public bool isAnim { get; private set; }

	public float AnimDuration
	{
		get
		{
			if (animSequence != null)
			{
				return animSequence.Duration(false);
			}
			return 0f;
		}
	}

	private Vector3 Position
	{
		get
		{
			if ((bool)rectTrans)
			{
				return rectTrans.anchoredPosition3D;
			}
			return trans.position;
		}
		set
		{
			if ((bool)rectTrans)
			{
				rectTrans.anchoredPosition3D = value;
			}
			else
			{
				trans.position = value;
			}
		}
	}

	private Vector3 LocalPosition
	{
		get
		{
			if ((bool)rectTrans)
			{
				return rectTrans.anchoredPosition3D;
			}
			return trans.localPosition;
		}
		set
		{
			if ((bool)rectTrans)
			{
				rectTrans.anchoredPosition3D = value;
			}
			else
			{
				trans.localPosition = value;
			}
		}
	}

	private void OnDestroy()
	{
		CleanUp();
	}

	private void OnEnable()
	{
		if (starOnEnable)
		{
			StartAnim();
		}
	}

	private void OnDisable()
	{
		if (starOnEnable)
		{
			StopAnim();
		}
	}

	private void Awake()
	{
		trans = base.transform;
		rectTrans = trans.GetComponent<RectTransform>();
		StopAnim();
	}

	private Sequence MakeSequence(List<AnimParameters> anims)
	{
		if (anims.Count <= 0)
		{
			return null;
		}
		Sequence sequence = DOTween.Sequence();
		sequence.SetAutoKill(false).SetUpdate(useIndependentTime).SetId(sequence.GetHashCode())
			.SetLoops(loopCount, loopType)
			.OnStart(AnimStarted)
			.OnComplete(AnimCompleted)
			.OnRewind(AnimRewinded);
		if (loopCount != 0)
		{
			float delay = UnityEngine.Random.Range(loopDelayFrom, loopDelayTo);
			sequence.SetDelay(delay);
		}
		foreach (AnimParameters anim in anims)
		{
			switch (anim.addType)
			{
			case AddType.Append:
				sequence.Append(MakeTwinner(anim));
				break;
			case AddType.Join:
				sequence.Join(MakeTwinner(anim));
				break;
			case AddType.Prepend:
				sequence.Prepend(MakeTwinner(anim));
				break;
			case AddType.Insert:
				sequence.Insert(anim.insertPosition, MakeTwinner(anim, sequence.Duration(false)));
				break;
			}
		}
		return sequence;
	}

	private Tweener MakeTwinner(AnimParameters ap, float sequenceDuration = 0f)
	{
		TweenParams tweenParams = new TweenParams().SetAutoKill(false).SetDelay(ap.startTime).SetEase(ap.animEase)
			.SetLoops(ap.loopCount, ap.loopType);
		Tweener tweener = null;
		float num = ((!(ap.duration > 0f)) ? (sequenceDuration - Mathf.Clamp(ap.insertPosition, 0f, ap.insertPosition) - Mathf.Clamp(ap.startTime, 0f, ap.startTime)) : ap.duration);
		switch (ap.animType)
		{
		case AnimType.LocalPosition:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.localPosition;
			}
			if (ap.overrideStartVal)
			{
				LocalPosition = ap.startValue;
			}
			tweener = ((!rectTrans) ? trans.DOLocalMove(ap.endValue, ap.durationIsSpeed ? (Vector3.Distance(ap.startValue, ap.endValue) / num) : num).SetAs(tweenParams) : rectTrans.DOAnchorPos3D(ap.endValue, ap.durationIsSpeed ? (Vector3.Distance(ap.startValue, ap.endValue) / num) : num).SetAs(tweenParams));
			break;
		case AnimType.LocalPositionX:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.localPosition;
			}
			if (ap.overrideStartVal)
			{
				LocalPosition = ap.startValue;
			}
			tweener = ((!rectTrans) ? trans.DOLocalMoveX(ap.endValue.x, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.x - ap.endValue.x) / num) : num).SetAs(tweenParams) : rectTrans.DOAnchorPosX(ap.endValue.x, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.x - ap.endValue.x) / num) : num).SetAs(tweenParams));
			break;
		case AnimType.LocalPositionY:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.localPosition;
			}
			if (ap.overrideStartVal)
			{
				LocalPosition = ap.startValue;
			}
			tweener = ((!rectTrans) ? trans.DOLocalMoveY(ap.endValue.y, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.y - ap.endValue.y) / num) : num).SetAs(tweenParams) : rectTrans.DOAnchorPosY(ap.endValue.y, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.y - ap.endValue.y) / num) : num).SetAs(tweenParams));
			break;
		case AnimType.LocalPositionZ:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.localPosition;
			}
			if (ap.overrideStartVal)
			{
				LocalPosition = ap.startValue;
			}
			tweener = ((!rectTrans) ? trans.DOLocalMoveZ(ap.endValue.z, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.z - ap.endValue.z) / num) : num).SetAs(tweenParams) : rectTrans.DOAnchorPos3D(ap.endValue, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.z - ap.endValue.z) / num) : num).SetAs(tweenParams));
			break;
		case AnimType.Position:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.position;
			}
			if (ap.overrideStartVal)
			{
				Position = ap.startValue;
			}
			tweener = ((!rectTrans) ? trans.DOMove(ap.endValue, ap.durationIsSpeed ? (Vector3.Distance(ap.startValue, ap.endValue) / num) : num).SetAs(tweenParams) : rectTrans.DOAnchorPos3D(ap.endValue, ap.durationIsSpeed ? (Vector3.Distance(ap.startValue, ap.endValue) / num) : num).SetAs(tweenParams));
			break;
		case AnimType.PositionX:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.position;
			}
			if (ap.overrideStartVal)
			{
				Position = ap.startValue;
			}
			tweener = ((!rectTrans) ? trans.DOMoveX(ap.endValue.x, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.x - ap.endValue.x) / num) : num).SetAs(tweenParams) : rectTrans.DOAnchorPosX(ap.endValue.x, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.x - ap.endValue.x) / num) : num).SetAs(tweenParams));
			break;
		case AnimType.PositionY:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.position;
			}
			if (ap.overrideStartVal)
			{
				Position = ap.startValue;
			}
			tweener = ((!rectTrans) ? trans.DOMoveY(ap.endValue.y, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.y - ap.endValue.y) / num) : num).SetAs(tweenParams) : rectTrans.DOAnchorPosY(ap.endValue.y, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.y - ap.endValue.y) / num) : num).SetAs(tweenParams));
			break;
		case AnimType.PositionZ:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.position;
			}
			if (ap.overrideStartVal)
			{
				Position = ap.startValue;
			}
			tweener = ((!rectTrans) ? trans.DOMoveZ(ap.endValue.z, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.z - ap.endValue.z) / num) : num).SetAs(tweenParams) : rectTrans.DOAnchorPos3D(ap.endValue, ap.durationIsSpeed ? (Mathf.Abs(ap.startValue.z - ap.endValue.z) / num) : num).SetAs(tweenParams));
			break;
		case AnimType.Scale:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.localScale;
			}
			if (ap.overrideStartVal)
			{
				trans.localScale = ap.startValue;
			}
			tweener = trans.DOScale(ap.endValue, ap.durationIsSpeed ? (Vector3.Distance(ap.startValue, ap.endValue) / num) : num).SetAs(tweenParams);
			break;
		case AnimType.RotateVector:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.localRotation.eulerAngles;
			}
			if (ap.overrideStartVal)
			{
				trans.localRotation = Quaternion.Euler(ap.startValue);
			}
			tweener = trans.DOLocalRotate(ap.endValue, ap.durationIsSpeed ? (Vector3.Distance(ap.startValue, ap.endValue) / num) : num, ap.rotateMode).SetAs(tweenParams);
			break;
		case AnimType.RotateQuaternion:
			if ((bool)ap.endValueTrans)
			{
				ap.endValue = ap.endValueTrans.localRotation.eulerAngles;
			}
			if (ap.overrideStartVal)
			{
				trans.localRotation = Quaternion.Euler(ap.startValue);
			}
			tweener = trans.DOLocalRotateQuaternion(Quaternion.Euler(ap.endValue), ap.durationIsSpeed ? (Vector3.Distance(ap.startValue, ap.endValue) / num) : num).SetAs(tweenParams);
			break;
		case AnimType.FadeImageOrGroup:
		{
			CanvasGroup component = trans.GetComponent<CanvasGroup>();
			if ((bool)component)
			{
				if (ap.overrideStartVal)
				{
					component.alpha = ap.startValue.z;
				}
				tweener = component.DOFade(ap.endValue.z, num).SetAs(tweenParams);
				break;
			}
			Image component2 = trans.GetComponent<Image>();
			if ((bool)component2)
			{
				Color color = component2.color;
				if (ap.overrideStartVal)
				{
					color.a = ap.startValue.z;
					component2.color = color;
				}
				color.a = ap.endValue.z;
				tweener = component2.DOColor(color, num).SetAs(tweenParams);
			}
			break;
		}
		case AnimType.ShakePosition:
			if (ap.overrideStartVal)
			{
				trans.position = ap.startValue;
			}
			tweener = trans.DOShakePosition(num, ap.shakeStrength, ap.shakeVibrato, ap.shakeRandomness).SetAs(tweenParams);
			break;
		case AnimType.ShakeRotation:
			if (ap.overrideStartVal)
			{
				trans.rotation = Quaternion.Euler(ap.startValue);
			}
			tweener = trans.DOShakeRotation(num, ap.shakeStrength, ap.shakeVibrato, ap.shakeRandomness).SetAs(tweenParams);
			break;
		case AnimType.ShakeScale:
			if (ap.overrideStartVal)
			{
				trans.localScale = ap.startValue;
			}
			tweener = trans.DOShakeScale(num, ap.shakeStrength, ap.shakeVibrato, ap.shakeRandomness).SetAs(tweenParams);
			break;
		}
		tweener.OnPlay(delegate
		{
			if (ap.OnStart != null)
			{
				ap.OnStart.Invoke();
			}
		});
		tweener.OnComplete(delegate
		{
			if (ap.OnEnd != null)
			{
				ap.OnEnd.Invoke();
			}
		});
		tweener.OnKill(delegate
		{
			if (ap.OnStopOrKill != null)
			{
				ap.OnStopOrKill.Invoke();
			}
		});
		if (DEBUG)
		{
			Debug.Log(string.Concat("MakeTwinner: ", tweener, ", ", ap.NameWithDetails()));
		}
		return tweener;
	}

	private void CleanUp()
	{
		if (DEBUG)
		{
			Debug.Log(string.Concat("CleanUp: ", tweens.Count, ", sequence: ", animSequence, ", ", animSequence == null));
		}
		if (animSequence != null)
		{
			isAnim = false;
			if (onLoopStopRewind)
			{
				animSequence.Rewind(false);
				animSequence.Kill();
			}
			else
			{
				animSequence.Kill(!onStopPlayBackwards);
			}
			animSequence = null;
		}
		if (tweens.Count <= 0)
		{
			return;
		}
		foreach (Tweener tween in tweens)
		{
			if (tween != null)
			{
				((Tween)tween).OnComplete((TweenCallback)null);
				tween.Kill(true);
			}
		}
		tweens.Clear();
	}

	public void StartAnim()
	{
		StartAnim(false);
	}

	public void StartAnim(bool now)
	{
		CleanUp();
		animSequence = MakeSequence(anims);
		animSequence.Complete(true);
	}

	public void StopAnim(bool now = false)
	{
		if (DEBUG)
		{
			Debug.Log("[MAGIK] StopAnim: " + base.name);
		}
		if (onStopPlayBackwards)
		{
			if (animSequence != null)
			{
				animSequence.Pause();
				if (now)
				{
					animSequence.Rewind(false);
					StopAnimAction();
					return;
				}
				isAnim = true;
				animSequence.SmoothRewind();
				if (OnRewind != null)
				{
					OnRewind.Invoke();
				}
			}
			else
			{
				StopAnimAction();
			}
		}
		else
		{
			StopAnimAction();
		}
	}

	private void StopAnimAction()
	{
		CleanUp();
		if (enableDisableObject)
		{
			base.gameObject.SetActive(false);
		}
		if (OnStop != null)
		{
			OnStop.Invoke();
		}
	}

	private void AnimStarted()
	{
		if (DEBUG)
		{
			Debug.Log("[MAGIK] AnimStarted: " + base.name + ", " + isAnim);
		}
		if (!isAnim)
		{
			isAnim = true;
			if (OnStart != null)
			{
				OnStart.Invoke();
			}
		}
	}

	private void AnimCompleted()
	{
		if (DEBUG)
		{
			Debug.Log("[MAGIK] AnimCompleted: " + base.name + ", " + isAnim);
		}
		if (isAnim)
		{
			isAnim = false;
			if (OnEnd != null)
			{
				OnEnd.Invoke();
			}
		}
	}

	private void AnimRewinded()
	{
		if (DEBUG)
		{
			Debug.Log("[MAGIK] AnimRewinded: " + isAnim + ", " + animSequence.IsBackwards());
		}
		if (isAnim && animSequence.IsBackwards())
		{
			StopAnimAction();
		}
	}
}
