using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private OnDropCard voltageDropArea;
    [SerializeField] private OnDropCard labelDropArea;
    [SerializeField] private OnDropCard doorPoweredDropArea;
    [SerializeField] private OnDropCard backupDropArea;
    [SerializeField] private OnDropCard printDropArea;
    [SerializeField] private OnDropCard statusConditionDropArea;
    [SerializeField] private OnDropCard batteryBranchDropArea;
    [SerializeField] private OnDropCard batteryConditionDropArea;
    [SerializeField] private OnDropCard inventoryIndexDropArea;
    [SerializeField] private OnDropCard hitFunctionDropArea;
    [SerializeField] private OnDropCard loopRangeDropArea;
    [SerializeField] private OnDropCard loopColonDropArea;
    [SerializeField] private OnDropCard conditionOperatorDropArea;
    [SerializeField] private OnDropCard conditionColonDropArea;
    [SerializeField] private OnDropCard coreLoopColonDropArea;
    [SerializeField] private OnDropCard coreConditionOperatorDropArea;
    [SerializeField] private OnDropCard coreConditionColonDropArea;
    [SerializeField] private OnDropCard corePowerDropArea;
    [SerializeField] private OnDropCard corePrintDropArea;
    [SerializeField] private OnDropCard coreMessageQuoteDropArea;
    [SerializeField] private OnDropCard corePrintMessageCloseDropArea;
    [SerializeField] private string conditionalStatus = "friend";
    [SerializeField] private int conditionalBattery = 30;
    [SerializeField] private TMP_Text resultText;

    public void OnRun()
    {
        if (SceneManager.GetActiveScene().name == "LevelTwo")
        {
            RunConditionalPuzzle();
            return;
        }

        if (SceneManager.GetActiveScene().name == "LevelThree")
        {
            RunInventoryPuzzle();
            return;
        }

        if (SceneManager.GetActiveScene().name == "LevelFour")
        {
            RunForLoopPuzzle();
            return;
        }

        if (SceneManager.GetActiveScene().name == "LevelFive")
        {
            RunSystemCorePuzzle();
            return;
        }

        if (voltageDropArea == null || labelDropArea == null ||
            doorPoweredDropArea == null || backupDropArea == null ||
            printDropArea == null)
        {
            Debug.LogError("GameManager is missing a drop-area reference.", this);
            return;
        }

        List<string> errors = new List<string>();
        string voltageValue = voltageDropArea.AssignedValue;
        string labelValue = labelDropArea.AssignedValue;
        string doorPoweredValue = doorPoweredDropArea.AssignedValue;
        string backupValue = backupDropArea.AssignedValue;
        string printValue = printDropArea.AssignedValue;

        if (voltageValue != "5" && voltageValue != "42")
        {
            errors.Add("voltage must be 5 or 42");
        }

        if (labelValue != "\"On\"")
        {
            errors.Add("label must be \"On\"");
        }

        if (doorPoweredValue != "True")
        {
            errors.Add("door_powered must be True");
        }

        if (backupValue != "42")
        {
            errors.Add("backup must be voltage + 42");
        }

        if (printValue != "label")
        {
            errors.Add("print() must print label");
        }

        if (errors.Count == 0)
        {
            const string successMessage = "Correct! The power grid is restored.";
            Debug.Log(successMessage, this);
            if (resultText != null)
            {
                resultText.text = successMessage;
            }
            SceneManager.LoadScene("LevelTwo");
            return;
        }

        string errorMessage = "Check your code: " + string.Join("; ", errors) + ".";
        Debug.Log(errorMessage, this);
        if (resultText != null)
        {
            resultText.text = errorMessage;
        }
    }

    private void RunInventoryPuzzle()
    {
        if (voltageDropArea == null || labelDropArea == null ||
            doorPoweredDropArea == null || backupDropArea == null ||
            printDropArea == null || inventoryIndexDropArea == null)
        {
            Debug.LogError("GameManager is missing an inventory drop-area reference.", this);
            return;
        }

        List<string> errors = new List<string>();
        string batteryValue = voltageDropArea.AssignedValue;
        string chipValue = labelDropArea.AssignedValue;
        string wireValue = doorPoweredDropArea.AssignedValue;
        string gearValue = backupDropArea.AssignedValue;
        string indexPrintValue = inventoryIndexDropArea.AssignedValue;
        string lengthPrintValue = printDropArea.AssignedValue;

        if (batteryValue != "battery")
        {
            errors.Add("the first inventory item must be battery");
        }

        if (chipValue != "chip")
        {
            errors.Add("the second inventory item must be chip");
        }

        if (wireValue != "wire")
        {
            errors.Add("the third inventory item must be wire");
        }

        if (gearValue != "gear")
        {
            errors.Add("append() must add gear");
        }

        if (indexPrintValue != "0")
        {
            errors.Add("the first print() must use index 0 to print inventory[0]");
        }

        if (lengthPrintValue != "len")
        {
            errors.Add("the second print() must print len(inventory)");
        }

        if (errors.Count > 0)
        {
            string errorMessage = "Check your code: " + string.Join("; ", errors) + ".";
            Debug.Log(errorMessage, this);
            if (resultText != null)
            {
                resultText.text = errorMessage;
            }
            return;
        }

        List<string> inventory = new List<string>
        {
            batteryValue,
            chipValue,
            wireValue
        };
        inventory.Add(gearValue);

        string successMessage = "Correct! print(inventory[0]) outputs " +
            inventory[0] + " and print(len(inventory)) outputs " + inventory.Count + ".";
        Debug.Log(successMessage, this);
        if (resultText != null)
        {
            resultText.text = successMessage;
        }
        SceneManager.LoadScene("LevelFour");
    }

    private void RunConditionalPuzzle()
    {
        if (statusConditionDropArea == null || batteryBranchDropArea == null ||
            batteryConditionDropArea == null)
        {
            Debug.LogError("GameManager is missing a conditional drop-area reference.", this);
            return;
        }

        List<string> errors = new List<string>();
        string statusOperator = statusConditionDropArea.AssignedValue;
        string batteryBranch = batteryBranchDropArea.AssignedValue;
        string batteryOperator = batteryConditionDropArea.AssignedValue;

        if (statusOperator != "==")
        {
            errors.Add("the status check must use ==");
        }

        if (batteryBranch != "elif")
        {
            errors.Add("the battery check must use elif");
        }

        if (batteryOperator != "<")
        {
            errors.Add("the battery check must use <");
        }

        if (errors.Count > 0)
        {
            string errorMessage = "Check your code: " + string.Join("; ", errors) + ".";
            Debug.Log(errorMessage, this);
            if (resultText != null)
            {
                resultText.text = errorMessage;
            }
            return;
        }

        string executedBranch;
        if (conditionalStatus == "Enemy")
        {
            executedBranch = "sound_alarm()";
        }
        else if (conditionalBattery < 50)
        {
            executedBranch = "recharge()";
        }
        else
        {
            executedBranch = "open_gate()";
        }

        string successMessage = "Correct! " + executedBranch + " runs.";
        Debug.Log(successMessage, this);
        if (resultText != null)
        {
            resultText.text = successMessage;
        }
        SceneManager.LoadScene("LevelThree");
    }

    private void RunForLoopPuzzle()
    {
        if (loopRangeDropArea == null || loopColonDropArea == null ||
            hitFunctionDropArea == null || conditionOperatorDropArea == null ||
            conditionColonDropArea == null)
        {
            Debug.LogError("GameManager is missing a for-loop drop-area reference.", this);
            return;
        }

        List<string> errors = new List<string>();
        if (loopRangeDropArea.AssignedValue != "range(10)")
        {
            errors.Add("the loop must iterate over range(10)");
        }

        if (loopColonDropArea.AssignedValue != ":")
        {
            errors.Add("the for loop must end with a colon");
        }

        if (hitFunctionDropArea.AssignedValue != "hit()")
        {
            errors.Add("hit() must run inside the loop");
        }

        if (conditionOperatorDropArea.AssignedValue != "==")
        {
            errors.Add("the last-hit condition must use ==");
        }

        if (conditionColonDropArea.AssignedValue != ":")
        {
            errors.Add("the if condition must end with a colon");
        }

        if (errors.Count > 0)
        {
            string errorMessage = "Check your code: " + string.Join("; ", errors) + ".";
            Debug.Log(errorMessage, this);
            if (resultText != null)
            {
                resultText.text = errorMessage;
            }
            return;
        }

        const string successMessage = "Correct! hit() runs 10 times, then smash() runs on i = 9.";
        Debug.Log(successMessage, this);
        if (resultText != null)
        {
            resultText.text = successMessage;
        }

        SceneManager.LoadScene("LevelFive");
    }

    private void RunSystemCorePuzzle()
    {
        if (coreLoopColonDropArea == null || coreConditionOperatorDropArea == null ||
            coreConditionColonDropArea == null || corePowerDropArea == null ||
            corePrintDropArea == null || coreMessageQuoteDropArea == null ||
            corePrintMessageCloseDropArea == null)
        {
            Debug.LogError("GameManager is missing a System Core drop-area reference.", this);
            return;
        }

        List<string> errors = new List<string>();
        if (coreLoopColonDropArea.AssignedValue != ":")
        {
            errors.Add("the for loop must end with a colon");
        }

        if (coreConditionOperatorDropArea.AssignedValue != "==")
        {
            errors.Add("the power condition must use ==");
        }

        if (coreConditionColonDropArea.AssignedValue != ":")
        {
            errors.Add("the if condition must end with a colon");
        }

        if (corePowerDropArea.AssignedValue != "100")
        {
            errors.Add("power must be set to 100");
        }

        if (corePrintDropArea.AssignedValue != "print")
        {
            errors.Add("the Core Online line must use print()");
        }

        if (coreMessageQuoteDropArea.AssignedValue != "\"")
        {
            errors.Add("the message string must be closed with a quote");
        }

        if (corePrintMessageCloseDropArea.AssignedValue != ")")
        {
            errors.Add("print(msg) must be closed with a parenthesis");
        }

        if (errors.Count > 0)
        {
            string errorMessage = "Check your code: " + string.Join("; ", errors) + ".";
            Debug.Log(errorMessage, this);
            if (resultText != null)
            {
                resultText.text = errorMessage;
            }
            return;
        }

        const string successMessage = "System Core is now stable, all leaks are patched, and you win!";
        Debug.Log(successMessage, this);
        if (resultText != null)
        {
            resultText.text = successMessage;
        }
    }
}
