using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
	START, 
	PLAYERTURN, 
	ENEMYTURN, 
	WON, 
	LOST
}

public class BattleSystem : MonoBehaviour
{
	// Battle state
	public BattleState state;

	// Player variables
	[SerializeField] private GameObject playerPrefab;
	[SerializeField] private Transform playerBattleStation;
	[SerializeField] private BattleHUD playerHUD;
	Unit playerUnit;
	[SerializeField] private GameObject attackButton = null;
	[SerializeField] private GameObject healButton = null;

	// Enemy variables
	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private Transform enemyBattleStation;
	[SerializeField] private BattleHUD enemyHUD;
	Unit enemyUnit;

	// Dialogue text
	public Text dialogueText;


	// Start is called before the first frame update
	void Start() 
	{
		// Set battle state to START
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }
	 
	// Function to set up the battle
	IEnumerator SetupBattle()
	{
		// Create an instance of the player on the player battle station
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();

		// Create an instance of the enemy on the enemy battle station
		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		// Set dialogue text for enemy approaching
		dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

		// Set up player and enemy HUDs
		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		// Pause for X number of second
		yield return new WaitForSeconds(2.0f);

		// Set battle state to PLAYERTURN
		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	// Player attack function
	IEnumerator PlayerAttack()
	{
		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "The attack is successful!";

		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		} 
		else 
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	// Enemy turn function
	IEnumerator EnemyTurn()
	{
		dialogueText.text = enemyUnit.unitName + " attacks!";

		yield return new WaitForSeconds(1f);

		bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

		playerHUD.SetHP(playerUnit.currentHP);

		yield return new WaitForSeconds(1f);

		if(isDead)
		{
			state = BattleState.LOST;
			EndBattle();
		} else
		{
			state = BattleState.PLAYERTURN;
			PlayerTurn();
		}

		// Reactivate attack and heal buttons after enemy turn
		attackButton.SetActive(true);
		healButton.SetActive(true);
	}

	// End battle function
	void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
	}

	// Player turn function
	void PlayerTurn()
	{
		// Set dialogue text to "Choose an action:"
		dialogueText.text = "Choose an action:";
	}

	// Player heal function
	IEnumerator PlayerHeal()
	{
		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}

	// Attack button pressed function
	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerAttack());

		// Deactivate attack and heal buttons after use
		attackButton.SetActive(false);
		healButton.SetActive(false);
	}

	// Heal button pressed function
	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerHeal());

		// Deactivate attack and heal buttons after use
		attackButton.SetActive(false);
		healButton.SetActive(false);
	}

}
