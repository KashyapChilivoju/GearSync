<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import Header from '../Header.svelte';

  let cars: any[] = [];  // Initialize as an empty array
  let isLoggedIn = false;
  let make = '';
  let model = '';
  let year: number | null = null;
  let colours = '';
  let body = '';
  let transmission = '';
  let fuelType = '';
  let seats: number | null = null;
  let doors: number | null = null;
  let isLoading = true; // Loading state

  // Check if the user is logged in
  onMount(() => {
    const token = localStorage.getItem('authToken');
    if (!token) {
      // If no authToken, redirect to login page
      goto('/login');
    }
    fetchCars();
  });

  async function fetchCars() {
    // Create the filter object dynamically, excluding default values
    const filters: any = {};

    if (make) filters.Make = make;
    if (model) filters.Model = model;
    if (year !== null) filters.Year = year;
    if (colours) filters.Colours = colours;
    if (body) filters.Body = body;
    if (transmission) filters.Transmission = transmission;
    if (fuelType) filters.FuelType = fuelType;
    if (seats !== null) filters.Seats = seats;
    if (doors !== null) filters.Doors = doors;

    // Log the filters to debug what is being sent in the request body
    console.log('Request Body:', JSON.stringify(filters));

    // Send a POST request to fetch cars with applied filters
    const response = await fetch('http://localhost:5187/cars/filter', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(filters), // Send only non-default filters
    });

    const data = await response.json();

    if (response.ok) {
      console.log('Cars fetched successfully:', data.cars);
      cars = data.cars; // Make sure cars is always an array
    } else {
      console.error('Error fetching cars:', data.Error);
    }

    // Simulate delay before rendering the fetched cars
    setTimeout(() => {
      isLoading = false; // Set loading state to false after a delay
    }, 500); // Delay of 500ms after fetching cars
  }

  function handleFilterChange() {
    // Call the function to fetch cars again after changing filters
    setTimeout(fetchCars, 500); // Adding a delay before updating the cars list after a filter change
  }
</script>

<main>
  <Header></Header>

  <section class="intro">
    <h1>Cars List</h1>

    <!-- Filter Options -->
    <div class="filters">
      <!-- Row 1 of filters -->
      <div class="filter-row">
        <input type="text" bind:value={make} placeholder="Make" on:input={handleFilterChange} />
        <input type="text" bind:value={model} placeholder="Model" on:input={handleFilterChange} />
        <input type="number" bind:value={year} placeholder="Year" on:input={handleFilterChange} />
        <input type="text" bind:value={colours} placeholder="Colours" on:input={handleFilterChange} />
        <input type="text" bind:value={body} placeholder="Body" on:input={handleFilterChange} />
      </div>

      <!-- Row 2 of filters -->
      <div class="filter-row">
        <input type="text" bind:value={transmission} placeholder="Transmission" on:input={handleFilterChange} />
        <input type="text" bind:value={fuelType} placeholder="Fuel Type" on:input={handleFilterChange} />
        <input type="number" bind:value={seats} placeholder="Seats" on:input={handleFilterChange} />
        <input type="number" bind:value={doors} placeholder="Doors" on:input={handleFilterChange} />
      </div>
    </div>

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
            </tr>
          </thead>
          <tbody>
            {#each cars as car}
              <tr>
                <td>{car.make}</td>
                <td>{car.model}</td>
                <td>{car.year}</td>
                <td>{car.colours}</td>
                <td>{car.transmission}</td>
                <td>{car.fuelType}</td>
                <td>{car.seats}</td>
                <td>{car.doors}</td>
              </tr>
            {/each}
          </tbody>
        </table>
      {:else}
        <p>No cars found matching your filters.</p>
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

  .filters {
    margin: 2rem 0;
    width: 100%;
  }

  .filter-row {
    display: flex;
    justify-content: space-between;
    gap: 1rem;
    margin-bottom: 1rem;
  }

  .filter-row input {
    padding: 0.5rem;
    font-size: 1rem;
    border-radius: 4px;
    border: 1px solid #ddd;
    flex: 1;
  }

  .cars-list {
    margin-top: 2rem;
    width: 100%;
    max-height: 362px;
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

  .cars-list p {
    margin: 0.5rem 0;
    font-size: 1rem;
    color: #333;
  }
</style>
