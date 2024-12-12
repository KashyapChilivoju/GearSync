<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import Header from './Header.svelte';
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

<Header></Header>

  <nav class="navbar">
    {#if isLoggedIn}
      <div class="user-actions">
        <p>Welcome, {username}!</p>
        <button on:click={logout}>Logout</button>
      </div>
      <ul>
        <li><a href="/add-car">Add/Remove Car</a></li>
        <li><a href="/list-cars">List Cars and Stock Levels</a></li>
        <li><a href="/update-stock">Update Car Stock Level</a></li>
        <li><a href="/search-car">Search Car by Make and Model</a></li>
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
    margin: 0; /* Removes margin to allow full width utilization */
    font-family: 'Arial', sans-serif;
  }

  :global(html, body) {
    margin: 0;
    padding: 0;
    width: 100%;
    height: 100%;
  }

  .header {
    display: flex;
    align-items: center;
    justify-content: space-between; /* Adjusts content spacing */
    width: 100%;
    gap: 0.5rem;
    padding: 1rem 0;
    border-bottom: 2px solid #ddd;
  }

  .logo {
    width: 200px; /* adjust logo size as necessary */
    height: auto;
  }

  h1 {
    flex-grow: 1;
    font-size: 2.5rem; /* adjusted for normal readability */
    color: #333;
    text-align: center;
  }

  .auth-actions, .user-actions {
    display: flex;
    gap: 1rem;
    align-items: center;
  }

  .auth-actions button, .user-actions button {
    padding: 0.5rem 1rem;
    font-size: 2rem;
    border: none;
    background-color: #007bff;
    color: white;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s;
  }

  .auth-actions button:hover, .user-actions button:hover {
    background-color: #0056b3;
  }

  .navbar {
    margin-top: 1rem;
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .navbar ul {
    list-style: none;
    padding: 0;
    display: flex;
    gap: 1rem;
    justify-content: center;
    flex-wrap: wrap;
  }

  .navbar ul li a, .navbar ul li button {
    text-decoration: none;
    color: #007bff;
    font-weight: bold;
    padding: 0.4rem 0.8rem;
    border-radius: 4px;
    transition: background-color 0.3s, color 0.3s;
  }

  .navbar ul li a:hover, .navbar ul li button:hover {
    text-decoration: none;
    background-color: #007bff;
    color: white;
  }
</style>

