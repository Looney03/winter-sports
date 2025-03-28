using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;

public class DeliverySystem : MonoBehaviour
{

    [SerializeField] private InputAction deliverYellowPresent;
    [SerializeField] private InputAction deliverBluePresent;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TextMeshProUGUI endScreenText;
    [SerializeField] private GameObject playerController;
    [SerializeField] private ParticleSystem presentEffectYellow;
    [SerializeField] private ParticleSystem presentEffectBlue;
    [SerializeField] private TextMeshProUGUI deliveryPromptText;


    private House currentHouse;
    private int deliveredPresents = 0;
    private int incorrectPresents = 0;
    private int totalHouses = 0;
    private int totalScore = 0;
    private bool gameEnded = false;

    private void OnEnable() 
    {
        Debug.Log("DeliverySystem Enabled");
        deliverYellowPresent.Enable();
        deliverBluePresent.Enable();

        deliverYellowPresent.performed += ctx => 
        {
            Debug.Log("Yellow Present Delivered Input Detected");
            DeliverPresent("yellow");
        };
        deliverBluePresent.performed += ctx => 
        {
            Debug.Log("Blue Present Delivered Input Detected");
            DeliverPresent("blue");
        };
    }

    private void OnDisable() 
    {
        Debug.Log("DeliverySystem Disabled");
        deliverYellowPresent.Disable();
        deliverBluePresent.Disable();
    }

    private void Update() 
    {
        if (gameEnded && playerController != null)
        {
            playerController.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            playerController.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            playerController.GetComponent<PresentsPlayerController>().enabled = false; 
        }

    }

    private void Start() 
    {
        
        House[] houses = FindObjectsOfType<House>();
        totalHouses = houses.Length;

        Debug.Log($"Total houses counted: {totalHouses}");
        foreach (var house in houses)
        {
            Debug.Log($"House found: {house.name}, Tag: {house.tag}");
        }
        
        UpdateScoreUI();
    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log($"Object entered trigger: {other.name}");

        if (other.CompareTag("House"))
        {
            House house = other.GetComponent<House>();
            if (house != null)
            {
                if (!house.isDelivered)
                {
                    Debug.Log($"House detected: {house.name}, Required Present: {house.presentColour}");
                    house.EnableOutline();
                    currentHouse = house;

                    if (deliveryPromptText != null)
                    {
                        if (house.presentColour == "yellow")
                        {
                            deliveryPromptText.text = "Deliver <b><color=yellow>Yellow</color></b> Present";
                        }
                        else if (house.presentColour == "blue")
                        {
                            deliveryPromptText.text = "Deliver <b><color=blue>Blue</color></b> Present";
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"{house.name} has already received a present.");
                }
            }
            else
            {
                Debug.LogWarning($"Collider tagged as House but missing House component: {other.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        Debug.Log($"Object exited trigger: {other.name}");
        if (other.CompareTag("House"))
        {
            House house = other.GetComponent<House>();
            if (house != null)
            {
                Debug.Log($"Exited House: {house.name}");
                house.DisableOutline();
            }
            else
            {
                Debug.LogWarning($"Collider tagged as House but missing House component: {other.name}");
            }

            if (deliveryPromptText != null)
            {
                deliveryPromptText.text = "";
            }

            currentHouse = null;


            currentHouse = null;
        }
    }

    private void DeliverPresent(string presentColour)
    {
        Debug.Log($"Attempting to deliver present: {presentColour}");

        if (currentHouse != null && !currentHouse.isDelivered)
        {
            if (currentHouse.presentColour == presentColour )
            {
                Debug.Log($"Correct present delivered to {currentHouse.name}!");
                deliveredPresents++;
                totalScore += 10;
            }
            else
            {
                Debug.Log($"Incorrect present delivered to {currentHouse.name}!");
                incorrectPresents++;
                totalScore = Mathf.Max(0, totalScore - 10);
            }


            if (presentColour == "yellow" && presentEffectYellow != null)
            {
                presentEffectYellow.Stop();
                presentEffectYellow.Play();
            }
            else if (presentColour == "blue" && presentEffectBlue != null)
            {
                presentEffectBlue.Stop();
                presentEffectBlue.Play();
            }
            else
            {
                Debug.LogWarning("No present effect assigned!");
            }

        
            currentHouse.MarkAsDelivered();

            if (deliveryPromptText != null)
            {
                deliveryPromptText.text = "";
            }

            currentHouse = null;

            UpdateScoreUI();
        }
        else if (currentHouse != null && currentHouse.isDelivered)
        {
            Debug.LogWarning($"Present already delivered to {currentHouse.name}.");
        }
        else
        {
            Debug.LogWarning("No house detected to deliver a present");
        }
    }


    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            int totalPresentsDelivered = deliveredPresents + incorrectPresents;
            Debug.Log($"Updating Score UI: {totalPresentsDelivered} / {totalHouses}");
            scoreText.text = $"Presents: {totalPresentsDelivered} / {totalHouses}\n" + $"Score: {totalScore}";

            if (totalPresentsDelivered >= totalHouses)
            {
                ShowEndScreen();
            }
        }
        else
        {
            Debug.LogWarning("ScoreText is not assigned in the Inspector");
        }

        
    }

    public int GetDeliveredPresentsCount()
    {
        return deliveredPresents;
    }

    public int GetTotalHousesCount()
    {
        return totalHouses;
    }

    private void ShowEndScreen()
    {
        gameEnded = true;
        if (endScreenPanel != null && endScreenText != null)
        {
            endScreenPanel.SetActive(true);
            endScreenText.text = $"Total Score: {totalScore}\n" +
                                $"Correct Deliveries: {deliveredPresents}\n" +
                                $"Incorrect Deliveries: {incorrectPresents}";

            deliverYellowPresent.Disable();
            deliverBluePresent.Disable();
        }
        else
        {
            Debug.LogWarning("End Screen Panel or Text is not assigned in the Inspector.");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    

}
