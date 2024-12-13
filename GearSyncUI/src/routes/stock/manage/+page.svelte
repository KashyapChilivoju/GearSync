<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import Header from '../../Header.svelte';

  let stock: any[] = []; // Initialize as an empty array
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
  let selectedCarID: number | null = null;  // Track the car being edited
  let selectedColour = '';
  let editableStockLevel: number | null = null; // Editable stock level for the selected car
  let errorMessage = '';
  let successMessage = '';

  // Check if the user is logged in
  onMount(() => {
    const token = localStorage.getItem('authToken');
    const dealerID = localStorage.getItem('dealerID');
    if (!token || !dealerID) {
      // If no authToken or dealerID, redirect to login page
      goto('/login');
    }
    fetchStock(dealerID, token); // Fetch stock list using dealer credentials
  });

  // Fetch stock based on dealer ID
  async function fetchStock(dealerID: string, token: string) {
    try {
      const response = await fetch('http://localhost:5187/stock/dealer', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          DealerID: dealerID,
          Token: token,
        }),
      });

      const data = await response.json();

      if (response.ok) {
        stock = data.stock; // Update the stock array
      } else {
        errorMessage = 'Error fetching stock.';
      }
    } catch (error) {
      errorMessage = 'Error fetching stock data.';
    } finally {
      isLoading = false;
    }
  }

  // Delete stock item (car)
  async function handleDeleteStock(carID: number) {
    const confirmDelete = window.confirm(`Are you sure you want to delete this car? This action cannot be undone.`);
    if (confirmDelete) {
      try {
        const response = await fetch('http://localhost:5187/stock/remove', {
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
          alert('Car removed from stock successfully!');
          fetchStock(localStorage.getItem('dealerID')!, localStorage.getItem('authToken')!); // Refresh stock list
        } else {
          alert(data.error || 'Failed to remove car from stock.');
        }
      } catch (error) {
        alert('An error occurred while removing car from stock.');
        console.error(error);
      }
    }
  }

  // Edit stock level
  function handleEditStock(car: any) {
    selectedCarID = car.carID;
    editableStockLevel = car.stockLevel; // Pre-fill with the current stock level
    console.log("THE handle editstock COLOUR IS: ", car.color);
    selectedColour = car.colour;
  }

  // Confirm stock level update
  async function handleUpdateStock() {
    if (editableStockLevel === null || editableStockLevel < 0) {
      alert('Please enter a valid stock level.');
      return;
    }

    const confirmUpdate = window.confirm(`Are you sure you want to update the stock level to ${editableStockLevel}?`);

    if (confirmUpdate) {
      try {
        if (editableStockLevel === 0) {
          // If stock level is 0, call RemoveStockEndpoint
          await handleDeleteStock(selectedCarID!);
        } else {
          // Update stock level

          console.log("THE SELECTED COLOUR IS: ", selectedColour);
          const response = await fetch('http://localhost:5187/stock/update', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
              CarID: selectedCarID,
              DealerID: localStorage.getItem('dealerID'),
              Token: localStorage.getItem('authToken'),
              StockLevel: editableStockLevel,
              Colour: selectedColour
            }),
          });

          const data = await response.json();

          if (response.ok) {
            alert('Stock level updated successfully!');
            fetchStock(localStorage.getItem('dealerID')!, localStorage.getItem('authToken')!); // Refresh stock list
          } else {
            alert(data.error || 'Failed to update stock level.');
          }
        }
      } catch (error) {
        alert('An error occurred while updating stock.');
        console.error(error);
      } finally {
        selectedCarID = null; // Reset the editing state
        editableStockLevel = null; // Reset the editable stock level
      }
    }
  }

  // Handle cancel edit stock
  function handleCancelEdit() {
    selectedCarID = null; // Cancel edit for the selected car
    editableStockLevel = null; // Clear the input
  }

  // Validate if form is complete and ready to submit
  $: isFormValid = selectedCarID !== null && editableStockLevel !== null;
</script>

<main>
  <Header></Header>

  <section class="intro">
    <h1>Manage Current Stock</h1>

    <!-- Stock List -->
    <div class="cars-list">
      {#if isLoading}
        <p>Loading stock...</p>
      {:else if stock.length > 0}
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
              <th>Stock Level</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {#each stock as car}
              <tr>
                <td>{car.make}</td>
                <td>{car.model}</td>
                <td>{car.year}</td>
                <td>{car.colour}</td>
                <td>{car.transmission}</td>
                <td>{car.fuelType}</td>
                <td>{car.seats}</td>
                <td>{car.doors}</td>
                {#if selectedCarID === car.carID}
                  <td>
                    <input type="number" bind:value={editableStockLevel} min="0" style="width: 50px;" id="stock-level-{car.carID}" name="stock-level-{car.carID}" />
                  </td>
                  <td>
                    <button on:click={handleUpdateStock}>Confirm</button>
                    <button on:click={handleCancelEdit}>Cancel</button>
                  </td>
                {:else}
                  <td>{car.stockLevel}</td>
                  <td>
                    <button on:click={() => handleEditStock(car)}>Edit</button>
                    <button on:click={() => handleDeleteStock(car.carID)}>X</button>
                  </td>
                {/if}
              </tr>
            {/each}
          </tbody>
        </table>
      {:else}
        <p>No stock found.</p>
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

  .cars-list {
    margin-top: 2rem;
    width: 100%;
    max-height: 500px;
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

  button:disabled {
    background-color: #ccc;
  }

  .success {
    color: green;
  }

  .error {
    color: red;
  }
</style>
