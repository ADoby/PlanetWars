using UnityEngine;

namespace SimpleLibrary
{
	//Monobehaviour Singleton for Unity use
	public class Singleton<ChildType> : MonoBehaviour where ChildType : MonoBehaviour
	{
		//Never use this directly, thats why its private NOT protected
		private static ChildType instance = null;

		//Use this to get the current single instance of this type
		public static ChildType Instance
		{
			get
			{
				if (instance == null)
					instance = FindObjectOfType<ChildType>();
				if (instance == null)
				{
					Debug.Log(string.Format("There was no object of type <{0}>", typeof(ChildType)));
				}
				return (ChildType)instance;
			}
			set
			{
				instance = value;
			}
		}

		protected virtual void Awake()
		{
			Instance = this as ChildType;
		}
	}

	public class AutoCreateSingleton : MonoBehaviour
	{
		public virtual void InitilizeSingleton()
		{
		}
	}

	//Monobehaviour Singleton for Unity use
	public class AutoCreateSingleton<ChildType> : AutoCreateSingleton where ChildType : AutoCreateSingleton
	{
		//Never use this directly, thats why its private NOT protected
		private static ChildType instance = null;

		//Use this to get the current single instance of this type
		public static ChildType Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<ChildType>();

					if (instance == null)
					{
						var go = new GameObject(typeof(ChildType).ToString());
						instance = go.AddComponent<ChildType>();
						instance.InitilizeSingleton();
					}
				}
				return instance;
			}
			set
			{
				instance = value;
			}
		}

		protected virtual void Awake()
		{
			instance = this as ChildType;
		}
	}
}