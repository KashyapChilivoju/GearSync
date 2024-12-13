<script lang="ts">
  import { goto } from '$app/navigation';
  import { onMount } from 'svelte';
  import Header from '../../Header.svelte';

  let makes: string[] = [];
  let models: string[] = []; // Make sure models is a string[] type
  let years: number[] = [];
  let colours: string[] = [];
  let carID: number | null = null;
  let selectedMake = '';
  let selectedModel = '';
  let selectedYear: number | null = null;
  let selectedColour = '';
  let stockLevel: number | null = null;
  let isLoading = true;
  let isFormValid = false;
  let errorMessage = '';
  let successMessage = '';

  // Check if the user is logged in
  onMount(() => {
    const token = localStorage.getItem('authToken');
    if (!token) {
      // If no authToken, redirect to login page
      goto('/login');
    }
    fetchMakes();
  });

  // Fetch car makes
  async function fetchMakes() {
    try {
      const response = await fetch('http://localhost:5187/cars');
      const data = await response.json();

      if (response.ok) {
        makes = data.cars.map((car: any) => car.make).filter((value, index, self) => self.indexOf(value) === index); // Ensure unique makes
      } else {
        errorMessage = 'Error fetching car makes.';
      }
    } catch (error) {
      errorMessage = 'Error fetching car data.';
    } finally {
      isLoading = false;
    }
  }

  // Fetch models based on selected make
  async function fetchModels() {
    if (selectedMake) {
      try {
        const response = await fetch('http://localhost:5187/cars/filter', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ Make: selectedMake }) // Send Make to fetch models
        });
        const data = await response.json();

        if (response.ok) {
          models = Array.from(new Set(data.cars.filter((car: any) => car.make === selectedMake).map((car: any) => car.model))); // Ensure unique models
        } else {
          errorMessage = 'Error fetching models for the selected make.';
        }
      } catch (error) {
        errorMessage = 'Error fetching models.';
      }
    }
  }

  // Fetch years based on selected model
  async function fetchYears() {
    if (selectedModel) {
      try {
        const response = await fetch('http://localhost:5187/cars/filter', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ Make: selectedMake, Model: selectedModel }) // Send Make and Model to fetch years
        });
        const data = await response.json();

        if (response.ok) {
          years = Array.from(new Set(data.cars.filter((car: any) => car.model === selectedModel).map((car: any) => car.year)));
        } else {
          errorMessage = 'Error fetching years for the selected model.';
        }
      } catch (error) {
        errorMessage = 'Error fetching years.';
      }
    }
  }

  // Fetch colours based on the selected car (make, model, year)
async function fetchColours() {
  if (selectedYear) {
    try {
      const response = await fetch('http://localhost:5187/cars/filter', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Make: selectedMake, Model: selectedModel, Year: selectedYear }) // Send Make, Model, Year to fetch colours
      });
      const data = await response.json();

      if (response.ok) {
        // Flatten the colours array, split the colours by ', ', and ensure uniqueness
        colours = Array.from(new Set(
          data.cars
            .filter((car: any) => car.model === selectedModel && car.year === selectedYear)
            .map((car: any) => car.colours) // Get the colours string
            .join(',') // Join all colours into one string if needed
            .split(',') // Split the string into individual colours based on ', '
        ));
      } else {
        errorMessage = 'Error fetching colours for the selected car.';
      }
    } catch (error) {
      errorMessage = 'Error fetching colours.';
    }
  }
}

// Fetch CarID based on the selected make, model, year, and colour
async function fetchCarID() {
  if (selectedMake && selectedModel && selectedYear && selectedColour) {
    try {
      const response = await fetch('http://localhost:5187/cars/filter', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          Make: selectedMake,
          Model: selectedModel,
          Year: selectedYear
        })
      });
      const data = await response.json();

      if (response.ok && data.cars && data.cars.length > 0) {
        carID = data.cars[0].carID;  // Set the CarID from the response
        console.log("CAR ID Is ", carID); // this is logging correctly
      } else {
        errorMessage = 'Car not found for the selected details.';
      }
    } catch (error) {
      errorMessage = 'Error fetching car ID.';
      console.error(error);
    }
  }
}

  // Handle form submission
  async function handleAddStock() {
    if (!selectedMake || !selectedModel || !selectedYear || !selectedColour || !stockLevel) {
      errorMessage = 'All fields are required.';
      return;
    }

    const token = localStorage.getItem('authToken');
    const dealerID = Number(localStorage.getItem('dealerID'));

    await fetchCarID();

    const requestBody = {
      CarID: carID,
      DealerID: dealerID,
      Token: token,
      Colour: selectedColour,
      StockLevel: stockLevel
    };
    console.log(requestBody); /// problem here no carID is being set

    try {
      const response = await fetch('http://localhost:5187/stock/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(requestBody),
      });

      const data = await response.json();

      if (response.ok) {
        successMessage = 'Stock added successfully!';
      } else {
        errorMessage = data.Error || 'Failed to add stock.';
      }
    } catch (error) {
      errorMessage = 'An error occurred while adding stock.';
      console.error(error);
    }
  }

  // Validate if form is complete and ready to submit
  $: isFormValid = selectedMake && selectedModel && selectedYear && selectedColour && stockLevel > 0;

</script>

<main>
  <Header></Header>

  <section class="intro">
    <h1>Add Stock</h1>

    <!-- Form to add stock -->
    <div class="form-container">
      <div class="form-group">
        <label for="make">Make</label>
        <select id="make" bind:value={selectedMake} on:change={fetchModels} required>
          <option value="" disabled>Select Make</option>
          {#each makes as make}
            <option value={make}>{make}</option>
          {/each}
        </select>
      </div>

      <div class="form-group">
        <label for="model">Model</label>
        <select id="model" bind:value={selectedModel} on:change={fetchYears} required>
          <option value="" disabled>Select Model</option>
          {#each models as model}
            <option value={model}>{model}</option>
          {/each}
        </select>
      </div>

      <div class="form-group">
        <label for="year">Year</label>
        <select id="year" bind:value={selectedYear} on:change={fetchColours} required>
          <option value="" disabled>Select Year</option>
          {#each years as year}
            <option value={year}>{year}</option>
          {/each}
        </select>
      </div>

      <div class="form-group">
        <label for="colour">Colour</label>
        <select id="colour" bind:value={selectedColour} required>
          <option value="" disabled>Select Colour</option>
          {#each colours as colour}
            <option value={colour}>{colour}</option>
          {/each}
        </select>
        
      </div>

      <div class="form-group">
        <label for="stockLevel">Stock Quantity</label>
        <input type="number" id="stockLevel" bind:value={stockLevel} required placeholder="Enter stock level" />
      </div>

      <button type="button" on:click={handleAddStock} disabled={!isFormValid}>Add to Stock</button>

      {#if isLoading}
        <p>Loading...</p>
      {/if}

      {#if successMessage}
        <p class="success">{successMessage}</p>
      {/if}

      {#if errorMessage}
        <p class="error">{errorMessage}</p>
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

  .form-container {
    width: 100%;
    max-width: 400px;
    padding: 20px;
    border: 1px solid #ccc;
    border-radius: 5px;
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .form-group {
    display: flex;
    flex-direction: column;
  }

  select, input, button {
    padding: 0.8rem;
    font-size: 1rem;
    margin-top: 0.5rem;
  }

  button[type="button"] {
    background-color: #007bff;
    color: white;
    border: none;
    cursor: pointer;
    border-radius: 4px;
  }

  button[type="button"]:disabled {
    background-color: #ccc;
  }

  .success {
    color: green;
  }

  .error {
    color: red;
  }
</style>
