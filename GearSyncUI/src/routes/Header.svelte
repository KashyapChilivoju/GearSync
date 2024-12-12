<script lang="ts">
    import { goto } from "$app/navigation";

    let isLoggedIn = false;
    let dealerName = '';

    // Check if the user is logged in
    function checkLoginStatus() {
        const token = localStorage.getItem("authToken");
        const name = localStorage.getItem("dealerName");
        if (token && name) {
            isLoggedIn = true;
            dealerName = name;
        } else {
            isLoggedIn = false;
            dealerName = '';
        }
    }

    // Run the check when the component is mounted
    checkLoginStatus();

    // Function to navigate to the home page
    function navigateHome() {
        goto("/");
    }

    // Logout functionality
    function handleLogout() {
        // Clear any session data (localStorage, tokens, etc.)
        localStorage.removeItem("authToken");
        localStorage.removeItem("dealerID");
        localStorage.removeItem("dealerName");

        // Navigate to login page
        goto("/login");
    }
</script>

<main>
    <!-- Header Section -->
    <header class="header">
        <div class="logo-container" on:click={navigateHome} style="cursor: pointer;">
            <img src="/favicon.png" alt="GearSync Logo" class="logo" />
            <h1>GearSync</h1>
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
</main>

<style>
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
        justify-content: space-between; /* Space between logo and logout */
        width: 100%;
        border-bottom: 2px solid #ddd;
        padding: 1rem 2rem;
        box-sizing: border-box;
    }

    .logo-container {
        display: flex;
        align-items: center;
        gap: 0.5rem;
    }

    .logo {
        width: 250px;
        height: auto;
    }

    h1 {
        font-size: 7rem;
        color: #D8DBDE;
        margin: 0;
    }

    .user-actions {
        display: flex;
        align-items: center;
        gap: 1rem;
    }

    .greeting {
        font-size: 1.5rem;
        color: #D8DBDE;
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
