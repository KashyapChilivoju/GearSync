<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import Header from '../../Header.svelte';

  let cars: any[] = [];  // Initialize as an empty array
  let isLoggedIn = false;
  let isLoading = true; // Loading state
  let errorMessage = '';
  let successMessage = '';

  // Check if the user is logged in
  onMount(() => {
    const token = localStorage.getItem('authToken');
    if (!token) {
      // If no authToken, redirect to login page
      goto('/login');
    }
    fetchCars();
  });

  // Fetch cars based on applied filters
  async function fetchCars() {
    try {
      const response = await fetch('http://localhost:5187/cars/filter', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({}), // Fetch all cars without filters for now
      });

      const data = await response.json();

      if (response.ok) {
        cars = data.cars; // Ensure cars is always an array
      } else {
        errorMessage = 'Error fetching cars.';
      }
    } catch (error) {
      errorMessage = 'An error occurred while fetching cars.';
    } finally {
      isLoading = false;
    }
  }

  // Delete car from stock
  async function handleDeleteCar(carID: number) {
    const confirmDelete = window.confirm('Are you sure you want to delete this car? This action cannot be undone.');

    if (confirmDelete) {
      try {
        const response = await fetch('http://localhost:5187/cars/remove', {
          method: 'DELETE',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            CarID: carID,
            DealerID: localStorage.getItem('dealerID'),
            Token: localStorage.getItem('authToken'),
          }),
        });

        const data = await response.json();

        if (response.ok) {
          successMessage = 'Car removed from stock successfully!';
          alert(successMessage); // Show success popup
          setTimeout(() => {
            goto('/'); // Redirect to home page after success
          }, 1000);
        } else {

        }
      } catch (error) {
      }
    }
  }
</script>

<main>
  <Header></Header>

  <section class="intro">
    <h1>Manage Cars</h1>

    {#if errorMessage}
      <div class="error-message">{errorMessage}</div>
    {/if}

    {#if successMessage}
      <div class="success-message">{successMessage}</div>
    {/if}

    <!-- Cars List -->
    <div class="cars-list">
      {#if isLoading}
        <p>Loading cars...</p>
      {:else if cars.length > 0}
        <table>
          <thead>
            <tr>
              <th>Make</th>
              <th>Model</th>
              <th>Year</th>
              <th>Color</th>
              <th>Transmission</th>
              <th>Fuel Type</th>
              <th>Seats</th>
              <th>Doors</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {#each cars as car}
              <tr on:click={() => handleDeleteCar(car.carID)}>
                <td>{car.make}</td>
                <td>{car.model}</td>
                <td>{car.year}</td>
                <td>{car.colours}</td>
                <td>{car.transmission}</td>
                <td>{car.fuelType}</td>
                <td>{car.seats}</td>
                <td>{car.doors}</td>
                <td>
                  <button on:click={() => handleDeleteCar(car.carID)}>Delete</button>
                </td>
              </tr>
            {/each}
          </tbody>
        </table>
      {:else}
        <p>No cars found.</p>
      {/if}
    </div>
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

  .cars-list {
    margin-top: 2rem;
    width: 100%;
    max-height: 459px;
    overflow-y: auto; /* Add scroll bar */
  }

  table {
    width: 100%;
    border-collapse: collapse;
  }

  th, td {
    padding: 1rem;
    text-align: left;
    border: 1px solid #ddd;
  }

  th {
    background-color: #f4f4f4;
  }

  tbody tr:hover {
    background-color: #f1f1f1;
  }

  button {
    background-color: #007bff;
    color: white;
    border: none;
    padding: 0.5rem;
    cursor: pointer;
    border-radius: 4px;
  }

  .success-message {
    color: green;
    margin-bottom: 1rem;
  }

  .error-message {
    color: red;
    margin-bottom: 1rem;
  }
</style>
