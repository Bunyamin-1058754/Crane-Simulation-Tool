using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wxi.CraneSimulation.Core.Services
{
    public class FirebaseService
    {
        public static string firebaseClient = "https://wxi-cranesimulation-default-rtdb.europe-west1.firebasedatabase.app/";
        public static string firebaseSecret = "ad9wYiVprBQ7bCq2dcp7Vxr5gGZ2nG9cVYnuTM6g";
        public FirebaseClient client;

        public FirebaseService() 
        { 
            client = new FirebaseClient(firebaseClient, new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(firebaseSecret) });
        }
                                      
    }
}
