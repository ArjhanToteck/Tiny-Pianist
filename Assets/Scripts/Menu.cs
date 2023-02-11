using UnityEngine;

public class Menu : MonoBehaviour
{
	public Animator mainMenu;
    public Animator credits;
	public Animator levelSelect;

	public void EnterMainMenu()
	{
		mainMenu.SetTrigger("enter");
	}

	public void ExitMainMenu()
    {
        mainMenu.SetTrigger("exit");
	}

    public void EnterCredits()
    {
		credits.SetTrigger("enter");
	}

	public void ExitCredits()
	{
		credits.SetTrigger("exit");
	}

	public void EnterLevelSelect()
	{
		levelSelect.SetTrigger("enter");
	}

	public void ExitLevelSelect()
	{
		levelSelect.SetTrigger("exit");
	}
}
