using System;
using System.Collections.Generic;

internal class Program
{
    public abstract class User
    {
        protected int userId;
        public string Name { 
            get; set;
        }
        public string PhoneNumber {
            get; set; 
        }

        public void Register()
        {
            Console.Write("Enter user ID: ");
            userId = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter your name: ");
            Name = Console.ReadLine();

            Console.Write("Enter your Phone Number: ");
            PhoneNumber = Console.ReadLine();

            Console.WriteLine("User  registered successfully.");
        }

        public bool Login()
        {
            Console.Write("Enter user ID: ");
            return Convert.ToInt32(Console.ReadLine()) == userId;
        }
    }

    public class Rider : User
    {
        private List<string> rideHistory = new List<string>();
        public string Location { get; set; }
        public string Destination { get; set; }

        public void RequestRide()
        {
            Console.Write("Enter your current location: ");
            Location = Console.ReadLine();
            Console.Write("Enter your destination: ");
            Destination = Console.ReadLine();
            rideHistory.Add($"From {Location} to {Destination}");
            Console.WriteLine("Ride requested successfully.");
        }

        public void ViewRideHistory()
        {
            Console.WriteLine("Ride History:");
            foreach (var ride in rideHistory)
            {
                Console.WriteLine(ride);
            }
        }
    }

    public class Driver : User
    {
        public bool IsAvailable;
        private List<string> tripHistory = new List<string>();

        public void ToggleAvailability()
        {
            IsAvailable = !IsAvailable;
            Console.WriteLine(IsAvailable ? "Driver is now available." : "Driver is now unavailable.");
        }

        public void AcceptRide(Trip trip)
        {
            trip.DriverName = Name;
            trip.StartTrip();
            tripHistory.Add($"Accepted ride: From {trip.StartLocation} to {trip.Destination}");
            Console.WriteLine($"Driver {Name} accepted ride: From {trip.StartLocation} to {trip.Destination}");
        }

        public void ViewTripHistory()
        {
            Console.WriteLine("Trip History:");
            foreach (var trip in tripHistory)
            {
                Console.WriteLine(trip);
            }
        }
    }

    public class Trip
    {
        public int TripID {
            get; set;
        }
        public string RiderName {
            get; set; 
        }
        public string DriverName {
            get; set;
        }
        public string StartLocation {
            get; set;
        }
        public string Destination { 
            get; set; 
        }

        public double Fare;
        public bool IsActive;

        public void StartTrip()
        {
            IsActive = true;
            Console.WriteLine("Trip started.");
        }

        public void EndTrip()
        {
            IsActive = false;
            Console.WriteLine("Trip ended.");
        }

        public void CalculateFare(double distance)
        {
            Fare = distance * 1.5;
        }

        public void DisplayTripDetails()
        {
            Console.WriteLine($"Trip ID: {TripID}, Rider: {RiderName}, Driver: {DriverName}, Start: {StartLocation}," +
                $" Destination: {Destination}, Fare: {Fare}, Status: {(IsActive ? "In Progress" : "Completed")}");
        }
    }

    public class RideSharingSystem
    {
        public List<Rider> registeredRiders = new List<Rider>();
        public List<Driver> registeredDrivers = new List<Driver>();
        public List<Trip> availableTrips = new List<Trip>();
        public int tripCounter = 1;

        public void RegisterUser(User user)
        {
            if (user is Rider)
                registeredRiders.Add((Rider)user);

            else if (user is Driver)
                registeredDrivers.Add((Driver)user);
        }

        public void RequestRide(Rider rider)
        {
            rider.RequestRide();
            foreach (var driver in registeredDrivers)
            {
                if (driver.IsAvailable)
                {
                    var trip = new Trip
                    {
                        TripID = tripCounter++,
                        RiderName = rider.Name,
                        StartLocation = rider.Location,
                        Destination = rider.Destination
                    };

                    availableTrips.Add(trip);
                    Console.WriteLine("Ride requested successfully. Available drivers:");
                    driver.AcceptRide(trip);
                    return;
                }
            }
            Console.WriteLine("No available drivers at the moment.");
        }

        public void CompleteTrip(int tripID)
        {
            var trip = availableTrips.Find(t => t.TripID == tripID);
            if (trip != null)
            {
                trip.EndTrip();
                Console.WriteLine("Trip completed.");
            }

            else
            {
                Console.WriteLine("Trip not found.");
            }
        }

        public void DisplayAllTrips()
        {
            foreach (var trip in availableTrips)
            {
                trip.DisplayTripDetails();
            }
        }

        public void DisplayAvailableTrips()
        {
            foreach (var trip in availableTrips)
            {
                if (string.IsNullOrEmpty(trip.DriverName))
                {
                    trip.DisplayTripDetails();
                }
            }
        }
    }

    private static void Main()
    {
        RideSharingSystem rideSharingSystem = new RideSharingSystem();
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\nWelcome to the Ride-Sharing System:");
            Console.WriteLine("1. Register as Rider");
            Console.WriteLine("2. Register as Driver");
            Console.WriteLine("3. Request a Ride (Rider)");
            Console.WriteLine("4. Accept a Ride (Driver)");
            Console.WriteLine("5. Complete a Trip (Driver)");
            Console.WriteLine("6. View Ride History (Rider)");
            Console.WriteLine("7. View Trip History (Driver)");
            Console.WriteLine("8. Display All Trips");
            Console.WriteLine("9. Exit");
            Console.Write("Choose an option: ");

            int choice;

            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    var rider = new Rider();
                    rider.Register();
                    rideSharingSystem.RegisterUser(rider);
                    break;

                case 2:
                    var driver = new Driver();
                    driver.Register();
                    rideSharingSystem.RegisterUser(driver);
                    driver.ToggleAvailability();
                    break;

                case 3:
                    if (rideSharingSystem.registeredRiders.Count > 0)
                    {
                        rideSharingSystem.RequestRide(rideSharingSystem.registeredRiders[^1]);
                    }
                    else
                    {
                        Console.WriteLine("No registered riders available.");
                    }
                    break;

                case 4:
                    if (rideSharingSystem.registeredDrivers.Count > 0)
                    {
                        rideSharingSystem.DisplayAvailableTrips();
                        Console.Write("Enter Trip ID to accept: ");

                        if (int.TryParse(Console.ReadLine(), out int TripID))
                        {
                            var trip = rideSharingSystem.availableTrips.Find(t => t.TripID == TripID);

                            if (trip != null)
                            {
                                rideSharingSystem.registeredDrivers[^1].AcceptRide(trip);
                            }
                            else
                            {
                                Console.WriteLine("Trip not found.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid Trip ID.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No registered drivers available.");
                    }
                    break;

                case 5:
                    Console.Write("Enter Trip ID to complete: ");
                    if (int.TryParse(Console.ReadLine(), out int tripID))
                    {
                        rideSharingSystem.CompleteTrip(tripID);

                    }
                    else
                    {
                        Console.WriteLine("Invalid Trip ID.");
                    }
                    break;

                case 6:
                    if (rideSharingSystem.registeredRiders.Count > 0)
                    {
                        // Assume the last registered rider
                        rideSharingSystem.registeredRiders[^1].ViewRideHistory(); 
                    }
                    else
                    {
                        Console.WriteLine("No registered riders available.");
                    }
                    break;

                case 7:
                    if (rideSharingSystem.registeredDrivers.Count > 0)
                    {
                        // Assume the last registered driver
                        rideSharingSystem.registeredDrivers[^1].ViewTripHistory(); 
                    }
                    else
                    {
                        Console.WriteLine("No registered drivers available.");
                    }
                    break;

                case 8:
                    rideSharingSystem.DisplayAllTrips();
                    break;

                case 9:
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}