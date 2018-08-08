using UnityEngine;

public interface ISelectable
{
	int ISelectID { get; set; }
	int InstanceID { get; set; }
	Transform ISelectTransform { get; set; }
	void OnSelected();
	void OnDeselected();
}
