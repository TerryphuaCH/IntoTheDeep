using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//importing directives for authenticaton
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

using System.Text.RegularExpressions;


public class AuthManager : MonoBehaviour
{
    //Firebase references
    public FirebaseAuth auth;
    public DatabaseReference dbReference;

    //retrieve user text
    public TMP_InputField emailInput;
    public TMP_InputField emailInput4Login;
    public TMP_InputField passwordInput;
    public TMP_InputField passwordInput4Login;
    public TMP_InputField usernameInput;

    //setup buttons and UI
    public GameObject signUp8tn;
    public GameObject signInBtn;
    public GameObject forgetPasswordBtn;
    public GameObject signOutBtn;

    public TextMeshProUGUI errorMsgContent;

    private void Awake()
    {
        InitializeFirebase();
    }

    //handle firebase in start menu
    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public async void SignUpNewUser()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (ValidateEmail(email) && ValidatePassword(password))
        {

            Debug.Log("validate success");
            FirebaseUser newPlayer = await SignUpNewUserOnly(email, password);

            if (newPlayer != null)
            {
                string username = usernameInput.text;
                await CreateNewSimplePlayer(newPlayer.UserId, username, username, newPlayer.Email);
                await UpdatePlayerDisplayName(username);//update user's display name in authenticate service
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            errorMsgContent.text = "Error in Signing. Invalid Email or password";
            errorMsgContent.gameObject.SetActive(true);
        }
    }

    public async Task<FirebaseUser> SignUpNewUserOnly(string email, string password)
    {
        Debug.Log("Sign Up method...");
        FirebaseUser newPlayer = null;

        await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                if(task.Exception != null)
                {
                    string errorMsg = this.HandleSignUpError(task);
                    errorMsgContent.text = errorMsg;
                    Debug.Log("error in signing up " + errorMsg);
                    errorMsgContent.gameObject.SetActive(true);
                }

            }

                //Debug.LogError("sorry, there was an error creating your new Account, Error:" + task.Exception);


            else if (task.IsCompleted)
            {
                errorMsgContent.gameObject.SetActive(false);
                newPlayer = task.Result;
                Debug.LogFormat("New Player Details {0} {1}", newPlayer.UserId, newPlayer.Email);
            }
        });
        return newPlayer;
    }


    public async Task CreateNewSimplePlayer(string uuid, string displayName,
        string userName, string email)
    {
        SimpleGamePlayer newPlayer = new SimpleGamePlayer(displayName, userName, email);
        Debug.LogFormat("Player details : {0}", newPlayer.PrintPlayer());

        //root/players/$uuid
        await dbReference.Child("players/" + uuid).SetRawJsonValueAsync(newPlayer.SimpleGamePlayerToJson());

        //update auth player with new display name -> tagging along the username input field
        //UpdatePlayerDisplayName(displayName);
    }


    public string GetCurentUserDisplayName()
    {
        return auth.CurrentUser.DisplayName;
    }

    public async Task UpdatePlayerDisplayName(string displayName)
    {
        if (auth.CurrentUser != null)
        {
            UserProfile profile = new UserProfile
            {
                DisplayName = displayName
            };
            await auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was cancelled");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }
                Debug.Log("User profile updated successfully");
                Debug.LogFormat("Checking current user display name from auth {0}", GetCurentUserDisplayName());
            });
        }
    }

    public void SignInUser()
    {
        Debug.Log("Sign In method...");
        string email = emailInput4Login.text.Trim();
        string password = passwordInput4Login.text.Trim();

        if (ValidateEmail(email) && ValidatePassword(password))
        {

            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    string errorMsg = this.HandleSigninError(task);
                    errorMsgContent.text = errorMsg;
                    Debug.Log("Error in sign in " + errorMsg);
                    errorMsgContent.gameObject.SetActive(true);
                    //Debug.LogError("Sorry, there was an error creating your account, Error: " + task.Exception);
                }

                else if (task.IsCompleted)
                {
                    errorMsgContent.gameObject.SetActive(false);
                    FirebaseUser currentPlayer = task.Result;
                    Debug.LogFormat("Welcome to Elon Clicker {0} :: {1}", currentPlayer.UserId, currentPlayer.Email);
                    SceneManager.LoadScene(1);
                }
            });
        }
        else
        {
            errorMsgContent.text = "Error in Signing in. Invalid email / password";
            errorMsgContent.gameObject.SetActive(true);
        }
    }

    public void SignOutUser()
    {
        Debug.Log("Sign Out method...");
        if (auth.CurrentUser != null)
        {
            Debug.LogFormat("Auth user {0} {1}", auth.CurrentUser.UserId, auth.CurrentUser.Email);

            //get current index of a scene
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            auth.SignOut();
            if (currentSceneIndex != 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public void ForgetPassword()
    {
        string email = emailInput.text.Trim();

        auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Sorry, there was an sending password reset, ERROR: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Forget password email sent successfully...");
            }
        });
        Debug.Log("Forget Password method...");
    }

    public FirebaseUser GetCurrentUser()
    {
        return auth.CurrentUser;
    }

    /// <summary>
    /// Simple Client side email validation
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public bool ValidateEmail(string email)
    {
        bool isValid = false;
        //for all emails have @
        const string pattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
        const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

        if (email != "" && Regex.IsMatch(email, pattern, options))
        {
            isValid = true;
        }

        return isValid;
    }

    public bool ValidatePassword(string password)
    {
        bool isValid = false;

        //length of password need at least 6 characters
        if (password != "" && password.Length >= 6)
        {
            isValid = true;
        }
        return isValid;
    }

    public string HandleSignUpError(Task<FirebaseUser> task)
    {
        string errorMsg = "";
        
        if(task.Exception != null)
        {
            FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            errorMsg = "Sign up Fail\n";
                switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    errorMsg += "Email already in use.";
                    break;
                case AuthError.WeakPassword:
                    errorMsg += "Password is weak, Use at least 6 characters";
                    break;
                case AuthError.MissingPassword:
                    errorMsg += "Password is missing";
                    break;
                case AuthError.InvalidEmail:
                    errorMsg += "Invalid email used";
                    break;
                default:
                    errorMsg += "Issue in authentication: " + errorCode;
                    break;
            }
            Debug.Log("Error message " + errorMsg);

        }

        return errorMsg;
    }

    public string HandleSigninError(Task<FirebaseUser> task)
    {
        string errorMsg = "";

        if (task.Exception != null)
        {
            FirebaseException firebaseEx = task.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            errorMsg = "Sign in Fail\n";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    errorMsg += "Email already in use.";
                    break;
                case AuthError.WrongPassword:
                    errorMsg += "Password is wrong..!";
                    break;
                case AuthError.MissingPassword:
                    errorMsg += "Password is missing";
                    break;
                case AuthError.InvalidEmail:
                    errorMsg += "Invalid email used";
                    break;
                case AuthError.UserNotFound:
                    errorMsg += "User is not found";
                    break;
                default:
                    errorMsg += "Issue in authentication: " + errorCode;
                    break;
            }
            Debug.Log("Error message " + errorMsg);

        }

        return errorMsg;
    }

}

