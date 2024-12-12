<script lang="ts">
    import { onMount } from "svelte";
    import { goto } from "$app/navigation";
  
    let isLoggedIn = false;
    let dealerName = "";
  
    // Check if the user is logged in
    onMount(() => {
      const token = localStorage.getItem("authToken");
      const name = localStorage.getItem("dealerName");
      if (token && name) {
        isLoggedIn = true;
        dealerName = name;
      } else {
        isLoggedIn = false;
        dealerName = "";
      }
    });
  
    // Function to navigate to the home page
    function navigateHome() {
      goto("/");
    }
  
    // Logout functionality
    function handleLogout() {
    const dealerID = localStorage.getItem("dealerID");
    const token = localStorage.getItem("authToken");

    if (dealerID && token) {
      // Call the logout endpoint using DELETE request
      fetch('http://localhost:5187/logout', {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          DealerID: dealerID,
          Token: token
        }),
      })
        .then(response => response.json())
        .then(data => {
          if (data.success) {
            // Clear session data from localStorage
            localStorage.removeItem("authToken");
            localStorage.removeItem("dealerID");
            localStorage.removeItem("dealerName");

            // Redirect to login page after successful logout
            goto("/login");
          } else {
            console.error('Logout failed:', data.Error || 'Unknown error');
          }
        })
        .catch(error => {
          console.error('Logout error:', error);
        });
    }
      // If no session data exists, perform the normal logout process
      localStorage.removeItem("authToken");
      localStorage.removeItem("dealerID");
      localStorage.removeItem("dealerName");
      goto("/login");
  }
  </script>
  
  <main>
    <!-- Header Section -->
    <header class="header">
      <div class="logo-container" on:click={navigateHome} style="cursor: pointer;">
        <img src="/favicon.png" alt="GearSync Logo" class="logo" />
        <img src="/favicon1.png" alt="GearSync Gear" class="logo1" />
        <h1>earSyn</h1>
        <img src="/favicon2.png" alt="GearSync Gear" class="logo2" />
      </div>
  
      <!-- Greeting and Logout Section -->
      {#if isLoggedIn}
        <div class="user-actions">
          <p class="greeting">Hi, {dealerName}</p>
          <button class="logout-button" on:click={handleLogout}>
            Logout
          </button>
        </div>
      {/if}
    </header>

    <nav class="navbar">
      {#if isLoggedIn}
        <ul>
          <li><a href="/cars/add">Add Car</a></li>
          <li><a href="/cars/remove">Remove Car</a></li>
          <li><a href="/cars">Car List</a></li>
          <li><a href="/stock">Get Current Stock</a></li>
          <li><a href="/stock/add">Add Stock</a></li>
          <li><a href="/stock/manage">Remove/Update Stock</a></li>
        </ul>
      {:else}
        <div class="auth-actions">
          <button on:click={() => goto('/login')}>Login</button>
          <button on:click={() => goto('/register')}>Register</button>
        </div>
      {/if}
    </nav>

  </main>
  
  <style>
    main {
    display: flex;
    flex-direction: column;
    align-items: stretch;
    width: 100%;
    margin: 0;
    font-family: 'Arial', sans-serif;
  }

  .navbar {
    margin-top: 1rem;
    width: 100%;
    display: flex;
    justify-content: center;
    background-color: #041322; /* Dark background */
    padding: 1rem 0;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }

  .navbar ul {
    list-style: none;
    display: flex;
    gap: 2rem; /* Space between links */
    padding: 0;
    margin: 0;
  }

  .navbar ul li {
    display: inline-block;
  }

  .navbar ul li a {
    text-decoration: none;
    color: #D8DBDE; /* Light text for contrast */
    font-weight: 500;
    font-size: 1.2rem;
    padding: 0.5rem 1.2rem;
    border: 2px solid transparent;
    border-radius: 5px;
    transition: background-color 0.3s, color 0.3s, border 0.3s;
  }

  .navbar ul li a:hover {
    background-color: #007bff; /* Highlight on hover */
    color: white;
    border-color: #0056b3; /* Border to match hover state */
  }

  .auth-actions {
    display: flex;
    gap: 1rem;
  }

  .auth-actions button {
    padding: 0.5rem 1rem;
    font-size: 1.2rem;
    font-weight: bold;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    background-color: #007bff;
    color: white;
    transition: background-color 0.3s;
  }

  .auth-actions button:hover {
    background-color: #0056b3;
  }
    main {
      display: flex;
      flex-direction: column;
      align-items: center;
      font-family: "Arial", sans-serif;
      margin: 0;
      background-color: #041322;
      width: 100%;
    }
  
    .header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      width: 100%;
      border-bottom: 2px solid #ddd;
      padding: 1rem 2rem;
      box-sizing: border-box;
    }
    .logo1{
        width: 130px;

    }

    .logo2{
        width: 60px;
        padding-top: 20px;
    }
  
    .logo-container {
      display: flex;
      align-items: center;

    }
  
    .logo {
      width: 250px;
      height: auto;
    }
  
    h1 {
      font-size: 7rem;
      color: #d8dbde;
      margin: 0;
    }
  
    .user-actions {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
  
    .greeting {
      font-size: 1.5rem;
      color: #d8dbde;
      margin: 0;
    }
  
    .logout-button {
      padding: 0.5rem 1rem;
      font-size: 1rem;
      background-color: #ff4d4d;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      transition: background-color 0.3s;
    }
  
    .logout-button:hover {
      background-color: #cc0000;
    }
  </style>
  