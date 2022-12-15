using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class AuthManagers : MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference dbReference;


    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TextMeshProUGUI warningLoginText;
    public TextMeshProUGUI confirmLoginText;

    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TextMeshProUGUI warningRegisterText;

    public TextMeshProUGUI displayNameText;


    private void Awake()
    {
        //Check all dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Dependencies available it will initialize firebase
                InitializeFirebase();
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies" + dependencyStatus);
            }
        });

    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth:");
        //Set authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }


    private IEnumerator Login(string _email, string _password)
    {
        //Call Firebase auth Signin function
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            // Error Handling
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Email is Missing! ";
                    break;
                case AuthError.MissingPassword:
                    message = " Password is Missing! ";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong password !";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email Address";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User login and get result
            user = LoginTask.Result;
            Debug.LogFormat("User login successful: {0} ({1})", user.DisplayName, user.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged in";
            SceneManager.LoadScene("Gamescene");
        }
    }

    // This block of code below is used for registering and validation of user account creation
    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebase = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebase.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Password is missing";
                        break;
                    case AuthError.WeakPassword:
                        message = "Password is weak please set a stronger one";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "This email is already taken";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                user = RegisterTask.Result;

                FirebaseUser newPlayer = RegisterTask.Result;
                string userName = usernameRegisterField.text;
                string passWord = passwordRegisterField.text;
                string email = emailRegisterField.text;
                string displayName = displayNameText.text;
                CreateNewSimplePlayer(newPlayer.UserId, displayName, userName, passWord, email);
                Debug.LogFormat("New Player Details {0} {1} {2}  {3}", newPlayer.UserId, userName, passWord, email);


                if (user != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    var ProfileTask = user.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException FirebaseException = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)FirebaseException.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }


    public void UpdatePlayerDisplayName(string displayName)
    {
        if (auth.CurrentUser != null)
        {
            UserProfile profile = new UserProfile { DisplayName = displayName };
            auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
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
                Debug.Log("User profile update successfully");
                Debug.LogFormat("Checking current user display name from auth {0}", GetCurrentUserDisplayName());
                GetCurrentUserDisplayName();
            });
        }
    }


    public void CreateNewSimplePlayer(string uuid, string displayName, string userName, string passWord, string email)
    {
        SimpleGamePlayer newPlayer = new SimpleGamePlayer(userName, passWord, email);
        Debug.LogFormat("Player details: {0} ", newPlayer.PrintPlayer());
        dbReference.Child("players/" + uuid).SetRawJsonValueAsync(newPlayer.SimpleGamePlayerToJson());

        // update new player name with new display name
        UpdatePlayerDisplayName(displayName);
    }

    public string GetCurrentUserDisplayName()
    {
        displayNameText.text = auth.CurrentUser.DisplayName;
        return auth.CurrentUser.DisplayName;
    }


    public void Forgotpassword()
    {

        string email = emailLoginField.text.Trim();

        auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Sorry, there was an sending a password reset, ERROR: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Forget Password email sent successfully...");
            }
        });
        Debug.Log("Forget password method...");
    }

    public void SignOutUser()
    {
        Debug.Log("Sign Out method...");
        if (auth.CurrentUser != null)
        {
            Debug.LogFormat("Auth user {0} {1}", auth.CurrentUser.UserId, auth.CurrentUser.Email);

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            auth.SignOut();
            if (currentSceneIndex != 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public FirebaseUser GetCurrentUser()
    {
        return auth.CurrentUser;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

