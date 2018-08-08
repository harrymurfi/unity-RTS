using UnityEngine;
using UnityEngine.UI;

public class AttackCommand : Command
{
	Button btn;

	void Start()
	{
		tooltips = "Attack (Q)";
		btn = GetComponent<Button>();
		GetComponent<Button>().onClick.AddListener(Attack);
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Q))
		{
			btn.onClick.Invoke();
		}

//		if(!onAttackMode)
//		{
//			if(Input.GetKeyDown(KeyCode.A))
//			{
//				onAttackMode = true;
//				if(onAttackMode) btn.onClick.Invoke();
//			}
//		}
//		else
//		{
//			if(Input.GetKeyDown(KeyCode.Escape))
//			{
//				onAttackMode = false;
//			}
//		}
	}

	void Attack()
	{
		CommandController.instance.ExtentCommand("Cancel");
		Selection.instance.ChangeState(SelectionState.OnAttackMode);
	}
}
