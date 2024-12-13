<script lang="ts">
  import { goto } from '$app/navigation';
  import Header from '../Header.svelte';

  let dealerName = '';
  let password = '';
  let showPassword = false;

  let showSuccessModal = false; // Show success popup
  let dealerID = null; // Store the returned DealerID
  let errorMessage = ''; // Show error message popup
  let showErrorModal = false;

  async function handleRegister() {
    const requestData = { DealerName: dealerName, Password: password };

    try {
      const response = await fetch('http://localhost:5187/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestData),
      });

      const responseData = await response.json();

      if (!response.ok) {
        errorMessage = responseData.error || 'An unexpected error occurred.';
        showErrorModal = true;
      } else {
        dealerID = responseData.dealerID; // Capture DealerID
        showSuccessModal = true; // Show success modal
      }
    } catch (error) {
      errorMessage = 'Network error. Please try again later.';
      showErrorModal = true;
    }
  }

  function closeErrorModal() {
    showErrorModal = false;
    errorMessage = '';
  }

  function handleSuccessRedirect() {
    showSuccessModal = false;
    goto('/'); // Navigate to the root page
  }
</script>

<main style="height: 100vh; display: flex; flex-direction: column;">
  <Header></Header>
  <div class="form-container">
    <form on:submit|preventDefault={handleRegister}>
      <h1>Register</h1>
      <div class="form-group">
        <label for="dealerName">Dealer Name</label>
        <input type="text" id="dealerName" bind:value={dealerName} required />
      </div>
      <div class="form-group">
        <label for="password">Password</label>
        <input type={showPassword ? 'text' : 'password'} id="password" bind:value={password} required />
        <button type="button" on:click={() => (showPassword = !showPassword)}>
          {showPassword ? 'Hide' : 'Show'} Password
        </button>
      </div>
      <button type="submit">Register</button>

      <p class="link-text">
        Already have an account? 
        <a href="/login">Log in here</a>.
      </p>
    </form>
  </div>

  <!-- Success Popup Modal -->
  {#if showSuccessModal}
    <div class="modal-overlay">
      <div class="modal">
        <h2>Registration Successful</h2>
        <p>Your registration was successful. Your Dealer ID is <strong>{dealerID}</strong>.</p><p> Make sure to store this dealerID, otherwise you will not be able to log into your account.</p>
        <button on:click={handleSuccessRedirect}>OK</button>
      </div>
    </div>
  {/if}

  <!-- Error Popup Modal -->
  {#if showErrorModal}
    <div class="modal-overlay">
      <div class="modal">
        <h2>Registration Failed</h2>
        <p>{errorMessage}</p>
        <button on:click={closeErrorModal}>Close</button>
      </div>
    </div>
  {/if}
</main>

<style>
  main {
    width: 100%;
    flex-grow: 1;
  }

  .form-container {
    flex-grow: 1;
    display: flex;
    justify-content: center;
    align-items: flex-start;
    padding-top: 50px;
    width: 100%;
  }

  form {
    width: 100%;
    max-width: 400px;
    padding: 20px;
    border: 1px solid #ccc;
    border-radius: 5px;
    display: flex;
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    text-align: left;
  }

  input,
  button {
    padding: 0.8rem;
    font-size: 1rem;
    margin-top: 0.5rem;
  }

  button[type='submit'] {
    background-color: #007bff;
    color: white;
    cursor: pointer;
    border: none;
    border-radius: 4px;
  }

  button:hover[type='submit'] {
    background-color: #0056b3;
  }

  button[type='button'] {
    background: none;
    color: #007bff;
    cursor: pointer;
    border: none;
  }

  .link-text {
    margin-top: 1rem;
    font-size: 0.9rem;
    color: #666;
  }

  .link-text a {
    color: #007bff;
    text-decoration: none;
    font-weight: bold;
  }

  .link-text a:hover {
    text-decoration: underline;
  }

  /* Modal Styles */
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .modal {
    background: white;
    padding: 2rem;
    border-radius: 8px;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.3);
    text-align: center;
  }

  .modal h2 {
    margin: 0 0 1rem;
    font-size: 1.5rem;
    color: #333;
  }

  .modal p {
    margin: 0 0 1.5rem;
    font-size: 1.1rem;
    color: #666;
  }

  .modal button {
    padding: 0.5rem 1rem;
    font-size: 1rem;
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: background-color 0.3s;
  }

  .modal button:hover {
    background-color: #0056b3;
  }
</style>
