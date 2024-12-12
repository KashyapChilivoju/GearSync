<script lang="ts">
  import { goto } from '$app/navigation';
    import { onMount } from 'svelte';
  import Header from '../../Header.svelte';

  // Check if the user is logged in
  onMount(() => {
    const token = localStorage.getItem('authToken');
    if (!token) {
      // If no authToken, redirect to login page
      goto('/login');
    }
  });
</script>
  
  <main>
    <Header></Header>
  
    <section class="intro">
      <h1>Add Stock</h1>
  
    </section>
  </main>
  
  <style>
    
  </style>
  