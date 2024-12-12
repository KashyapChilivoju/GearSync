<script lang="ts">
  import Header from './Header.svelte';
</script>

<main>
  <Header></Header>

  <section class="intro">
    <h1>Welcome to GearSync</h1>
    <p>
      GearSync is a simple, yet efficient car dealership management application designed to streamline operations
      and enhance the user experience for car dealerships. This platform allows dealerships to manage their
      vehicle inventory, track stock levels, and perform basic operations like adding/removing cars and updating stock levels.
    </p>

    <p>
      The project is part of the <strong>OracleCMS hiring coding challenge</strong>, created to demonstrate my proficiency
      in software development, web technologies, and database management. With a focus on clean code and usability, GearSync
      is built to handle basic dealership operations and serves as a robust, user-friendly platform for managing car inventories.
    </p>

    <p>
      You can explore the source code for GearSync on GitHub: <a href="https://github.com/KashyapChilivoju/GearSync" target="_blank">GearSync GitHub Repository</a>
    </p>
  </section>
</main>

<style>
  main {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
    margin: 0;
    font-family: 'Arial', sans-serif;
  }

  .intro {
    padding: 2rem;
    text-align: center;
    max-width: 800px;
    width: 100%;
  }

  .intro h1 {
    font-size: 3rem;
    color: #333;
    margin-bottom: 1rem;
  }

  .intro p {
    font-size: 1.2rem;
    line-height: 1.6;
    color: #666;
    margin-bottom: 1rem;
  }

  .intro a {
    color: #007bff;
    text-decoration: none;
    font-weight: bold;
  }

  .intro a:hover {
    text-decoration: underline;
  }
</style>
