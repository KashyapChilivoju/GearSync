<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import Header from '../../Header.svelte';

  let make = '';
  let model = '';
  let year: number | null = null;
  let colours = '';
  let body = '';
  let transmission = '';
  let fuelType = '';
  let seats: number | null = null;
  let doors: number | null = null;

  let errorMessage = '';
  let successMessage = '';
  let isSubmitting = false;

  // Check if the user is logged in
  onMount(() => {
    const token = localStorage.getItem('authToken');
    const dealerID = localStorage.getItem('dealerID');
    if (!token || !dealerID) {
      goto('/login'); // Redirect to login page if not logged in
    }
  });

  // Handle form submission
  async function handleSubmit() {
    if (!isValid()) {
      return;
    }

    const token = localStorage.getItem('authToken');
    const dealerID = localStorage.getItem('dealerID');

    const requestBody = {
      Make: make,
      Model: model,
      Year: year,
      Colours: colours,
      Body: body,
      Transmission: transmission,
      FuelType: fuelType,
      Seats: seats,
      Doors: doors
    };

    isSubmitting = true;
    try {
      const response = await fetch('http://localhost:5187/cars/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestBody),
      });

      const data = await response.json();

      if (response.ok) {
        successMessage = 'Car added successfully!';
        clearForm();
        alert('Car added successfully!'); // Popup success message
        setTimeout(() => {
          goto('/');  // Redirect to home page after success
        }, 2000);
      } else {
        errorMessage = data.Error || 'Failed to add car.';
        alert(errorMessage); // Popup error message
      }
    } catch (error) {
      errorMessage = 'An error occurred while adding the car.';
      alert(errorMessage); // Popup error message
      console.error(error);
    } finally {
      isSubmitting = false;
    }
  }

  // Validate form fields
  function isValid() {
    if (!make || !model || !year || !colours || !body || !transmission || !fuelType || !seats || !doors) {
      errorMessage = 'Please fill in all fields.';
      return false;
    }

    if (year < 1886 || year > new Date().getFullYear() + 1) {
      errorMessage = 'Year must be between 1886 and the next year.';
      return false;
    }

    if (!['Sedan', 'Hatchback', 'SUV', 'Truck', 'Coupe', 'Convertible', 'Wagon', 'Van'].includes(body)) {
      errorMessage = 'Invalid body type.';
      return false;
    }

    if (!['Manual', 'Automatic', 'Semi-Automatic'].includes(transmission)) {
      errorMessage = 'Invalid transmission type.';
      return false;
    }

    if (!['Petrol', 'Diesel', 'Electric', 'Hybrid', 'CNG', 'LPG'].includes(fuelType)) {
      errorMessage = 'Invalid fuel type.';
      return false;
    }

    if (seats <= 0 || seats > 100) {
      errorMessage = 'Seats must be between 1 and 100.';
      return false;
    }

    if (doors <= 0 || doors > 10) {
      errorMessage = 'Doors must be between 1 and 10.';
      return false;
    }

    return true;
  }

  // Clear form fields after successful submission
  function clearForm() {
    make = '';
    model = '';
    year = null;
    colours = '';
    body = '';
    transmission = '';
    fuelType = '';
    seats = null;
    doors = null;
    errorMessage = '';
    successMessage = '';
  }
</script>

<main>
  <Header></Header>

  <section class="intro">
    <h1>Add a New Car</h1>

    {#if errorMessage}
      <div class="error-message">{errorMessage}</div>
    {/if}

    {#if successMessage}
      <div class="success-message">{successMessage}</div>
    {/if}

    <form on:submit|preventDefault={handleSubmit}>
      <div class="form-container">
        <div class="form-left">
          <div class="form-group">
            <label for="make">Make</label>
            <input type="text" id="make" bind:value={make} required />
          </div>

          <div class="form-group">
            <label for="model">Model</label>
            <input type="text" id="model" bind:value={model} required />
          </div>

          <div class="form-group">
            <label for="fuelType">Fuel Type</label>
            <select id="fuelType" bind:value={fuelType} required>
              <option value="Petrol">Petrol</option>
              <option value="Diesel">Diesel</option>
              <option value="Electric">Electric</option>
              <option value="Hybrid">Hybrid</option>
              <option value="CNG">CNG</option>
              <option value="LPG">LPG</option>
            </select>
          </div>

          <div class="form-group">
            <label for="transmission">Transmission</label>
            <select id="transmission" bind:value={transmission} required>
              <option value="Manual">Manual</option>
              <option value="Automatic">Automatic</option>
              <option value="Semi-Automatic">Semi-Automatic</option>
            </select>
          </div>

          <div class="form-group">
            <label for="body">Body Type</label>
            <select id="body" bind:value={body} required>
              <option value="Sedan">Sedan</option>
              <option value="Hatchback">Hatchback</option>
              <option value="SUV">SUV</option>
              <option value="Truck">Truck</option>
              <option value="Coupe">Coupe</option>
              <option value="Convertible">Convertible</option>
              <option value="Wagon">Wagon</option>
              <option value="Van">Van</option>
            </select>
          </div>
        </div>

        <div class="form-right">
          <div class="form-group">
            <label for="year">Year</label>
            <input type="number" id="year" bind:value={year} min="1886" max={new Date().getFullYear() + 1} required />
          </div>
          <div class="form-group">
            <label for="colours">Colours (comma separated)</label>
            <input type="text" id="colours" bind:value={colours} required />
          </div>

          <div class="form-group">
            <label for="seats">Seats</label>
            <input type="number" id="seats" bind:value={seats} min="1" max="100" required />
          </div>

          <div class="form-group">
            <label for="doors">Doors</label>
            <input type="number" id="doors" bind:value={doors} min="1" max="10" required />
          </div>
        </div>
      </div>

      <div class="form-group">
        <button type="submit" disabled={isSubmitting} class="submit-button">Add Car</button>
      </div>
    </form>
  </section>
</main>

<style>
  main {
    display: flex;
    flex-direction: column;
    align-items: center;
    font-family: 'Arial', sans-serif;
  }

  .intro {
    max-width: 900px;
    width: 100%;
    padding: 2rem;
    border-radius: 8px;
    background-color: #f9f9f9;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  }

  h1 {
    text-align: center;
    margin-bottom: 1rem;
  }

  .form-container {
    display: flex;
    justify-content: space-between;
    gap: 2rem;
  }

  .form-left, .form-right {
    width: 45%;
  }

  .form-group {
    margin-bottom: 1.2rem;
  }

  label {
    font-weight: bold;
  }

  input, select {
    width: 100%;
    padding: 0.8rem;
    margin-top: 0.5rem;
    font-size: 1rem;
    border: 1px solid #ddd;
    border-radius: 4px;
  }

  button {
    background-color: #007bff;
    color: white;
    padding: 0.8rem;
    font-size: 1.24rem;
    border: none;
    cursor: pointer;
    border-radius: 4px;
  }

  button:disabled {
    background-color: #ccc;
  }

  .submit-button {
    display: block;
    margin: 0 auto;
    width: 100%;
    max-width: 300px;
  }

  .error-message {
    color: red;
    margin-bottom: 1rem;
  }

  .success-message {
    color: green;
    margin-bottom: 1rem;
  }
</style>
