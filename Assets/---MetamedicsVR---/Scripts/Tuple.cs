using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Tuple<T1, T2>
{
	public T1 item1;
	public T2 item2;

	public Tuple(T1 item1, T2 item2)
	{
		this.item1 = item1;
		this.item2 = item2;
	}

	public static int IndexOfItem1(List<Tuple<T1, T2>> list, T1 item)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].item1.Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public static int IndexOfItem1(Tuple<T1, T2>[] array, T1 item)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].item1.Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public static int IndexOfItem2(List<Tuple<T1, T2>> list, T2 item)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].item2.Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public static int IndexOfItem2(Tuple<T1, T2>[] array, T2 item)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].item2.Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public override bool Equals(object o)
	{
		if (!(o is Tuple<T1, T2>)) return false;
		var other = (Tuple<T1, T2>)o;
		return this.Equals(other);
	}

	public bool Equals(Tuple<T1, T2> other)
	{
		if (ReferenceEquals(this, other)) return true;
		if (other is null) return false;
		if (this.item1 == null)
		{
			if (other.item1 == null)
			{
				if (this.item2 == null)
				{
					return other.item2 == null;
				}
				return this.item2.Equals(other.item2);
			}
			return false;
		}
		if (this.item2 == null)
		{
			if (other.item2 == null) return this.item1.Equals(other.item1);
			return false;
		}
		return this.item1.Equals(other.item1) && this.item2.Equals(other.item2);
	}

	public override int GetHashCode()
	{
		return item1.GetHashCode() + item2.GetHashCode();
	}

	public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
	{
		return !(a == b);
	}
}