<script lang="ts">
    import { onMount } from 'svelte';
    import { goto } from '$app/navigation';
  
    let isLoggedIn = false; // Initial state, checks cookie for user session
    let username = '';
  
    onMount(() => {
      // Check for user session (simplified cookie retrieval)
      const userSession = document.cookie.split('; ').find(row => row.startsWith('user='));
      if (userSession) {
        const sessionData = JSON.parse(decodeURIComponent(userSession.split('=')[1]));
        isLoggedIn = true;
        username = sessionData.username || '';
      }
    });
  
    function logout() {
      document.cookie = "user=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
      isLoggedIn = false;
      goto('/');
    }
  </script>
  
  <main>
    <header class="header">
      <img src="/favicon.png" alt="GearSync Logo" class="logo" />
      <h1>GearSync</h1>
    </header>
  
    {#if isLoggedIn}
      <nav class="navbar">
        <p>Welcome, {username}!</p>
        <button on:click={logout}>Logout</button>
        <ul>
          <li><a href="/add-car">Add/Remove Car</a></li>
          <li><a href="/list-cars">List Cars and Stock Levels</a></li>
          <li><a href="/update-stock">Update Car Stock Level</a></li>
          <li><a href="/search-car">Search Car by Make and Model</a></li>
        </ul>
      </nav>
    {:else}
      <div class="auth-actions">
        <button on:click={() => goto('/login')}>Login</button>
        <button on:click={() => goto('/register')}>Register</button>
      </div>
    {/if}
  </main>
  
  <style>
    main {
      display: flex;
      flex-direction: column;
      align-items: center;
      font-family: Arial, sans-serif;
      margin: 2rem;
    }
  
    .header {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
  
    .logo {
      width: 50px;
      height: 50px;
    }
  
    h1 {
      font-size: 2rem;
      color: #333;
    }
  
    .auth-actions,
    .navbar {
      margin-top: 2rem;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1rem;
    }
  
    .auth-actions button,
    .navbar button {
      padding: 0.5rem 1rem;
      font-size: 1rem;
      border: none;
      background-color: #007bff;
      color: white;
      border-radius: 4px;
      cursor: pointer;
    }
  
    .auth-actions button:hover,
    .navbar button:hover {
      background-color: #0056b3;
    }
  
    .navbar ul {
      list-style: none;
      padding: 0;
      display: flex;
      gap: 1rem;
    }
  
    .navbar ul li a {
      text-decoration: none;
      color: #007bff;
      font-weight: bold;
    }
  
    .navbar ul li a:hover {
      text-decoration: underline;
    }
  </style>
  