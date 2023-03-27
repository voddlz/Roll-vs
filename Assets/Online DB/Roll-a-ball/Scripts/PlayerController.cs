using UnityEngine;

// Include the namespace required to use Unity UI
using UnityEngine.UI;
using TMPro;

using System.Collections;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine.SceneManagement;
using UnityEditorInternal;

namespace Old
{
	/// <summary>
	/// This class has a couple of variations to implement the new mechanics, mostly keeping track of a timer in Update and a few changes to OnTriggerEnter to add a bit of extra time to the timer.
	/// </summary>
	public class PlayerController : MonoBehaviour
	{

        // Create public variables for player speed, and for the Text UI game objects
        public GameObject player;
        public float speed;
		public float timer = 30;
		public Text countText;
		public Text endText;
		public Text timerText;


		public TextMeshProUGUI nameInput;
		public TextMeshProUGUI passwordInput;
		public Button startGameButton;
		public Button restartGameButton;
		public Button quitGameButton;
		public TextMeshProUGUI titleText;
		public GameObject bgImg;

		// Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
        private string playerName = "";
        private string playerPassword = "";
		private Vector3 startPosition;
		private Rigidbody rb;
		private int count;
		private bool gameOver = true;

		public void StartGame()
		{
			playerName = nameInput.text;
			playerPassword = passwordInput.text;

            if (playerName != null && playerPassword != null)
			{
                nameInput.transform.parent.parent.gameObject.SetActive(false);
                passwordInput.transform.parent.parent.gameObject.SetActive(false); ;
                startGameButton.gameObject.SetActive(false);

                timerText.gameObject.SetActive(true);
                countText.gameObject.SetActive(true);
                endText.gameObject.SetActive(false);
				titleText.gameObject.SetActive(false);
				bgImg.SetActive(false);

                // Assign the Rigidbody component to our private rb variable
                rb = GetComponent<Rigidbody>();

				startPosition = player.transform.position;

                // Set the count to zero 
                count = 0;

                // Run the SetCountText function to update the UI (see below)
                SetCountText();

				// Set the text property of our Win Text UI to an empty string, making the 'You Win' (game over message) blank
				endText.gameObject.SetActive(false);
                endText.text = "";

                gameOver = false;
            }
            else
            {
				endText.gameObject.SetActive(true);
                endText.text = "Please enter a valid name and password to play";
            }
        }

		public void RestartGame()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Update()
        {
			// if the game is ongoing
			if (!gameOver)
            {
				//update timer and UI
				timer -= Time.deltaTime;
				timerText.text = timer.ToString("0.#");

				//if the timer runs out...
				if (timer <= 0)
                {
                    GameOver();
				}
			}
        }

		private void GameOver()
		{
			player.transform.position = startPosition;

            //set that the game is over
            gameOver = true;

            //update text
			endText.gameObject.SetActive(true);
			bgImg.SetActive(true);
			countText.gameObject.SetActive(false);
			timerText.gameObject.SetActive(false);
            endText.text = "Time ran out and you got a score of: " + count;

			StartCoroutine(Upload());
        }

		private IEnumerator Upload()
		{
            GetComponent<UploadToDB>().CreatePlayer(playerName, playerPassword, count);

            yield return new WaitForSeconds(2);

            GetComponent<UploadToDB>().GetHighScores();

			yield return new WaitForSeconds(2);

			restartGameButton.gameObject.SetActive(true);
			quitGameButton.gameObject.SetActive(true);
        }

        // Each physics step..
        void FixedUpdate()
		{
            if(!gameOver)
            { 
				// Set some local float variables equal to the value of our Horizontal and Vertical Inputs
                float moveHorizontal = Input.GetAxis("Horizontal");
                float moveVertical = Input.GetAxis("Vertical");

                // Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
                Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

                // Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
                // multiplying it by 'speed' - our public player speed that appears in the inspector
                rb.AddForce(movement * speed);
            }
			if (timer <= 0)
			{
                rb.velocity = Vector3.zero;
            }
        }

		// When this game object intersects a collider with 'is trigger' checked, 
		// store a reference to that collider in a variable named 'other'..
		void OnTriggerEnter(Collider other)
		{
			// ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
			if (other.gameObject.CompareTag("Pick Up"))
			{
				// Instead of just disabling the object forever, I added a new script with a coroutine that disables it and re-enables it after some time
				StartCoroutine(other.GetComponent<PickUp>().ReSpawn());

				// Add one to the score variable 'count'
				count = count + 1;

				// Add some more time to the timer 
				timer += 1;

				// Run the 'SetCountText()' function (see below)
				SetCountText();
			}
		}

		// Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
		void SetCountText()
		{
			// Update the text field of our 'countText' variable
			countText.text = "Count: " + count.ToString();
		}
	}
}