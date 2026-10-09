import { useState, useEffect } from 'react';
import { MovieCard } from './components/MovieCard';
import { MovieForm } from './components/MovieForm';

const API = 'http://localhost:5000/api/movies';

export default function App() {
  const [movies, setMovies] = useState([]);
  const [actors, setActors] = useState([]);
  const [editingMovie, setEditingMovie] = useState(null);
  const [loading, setLoading] = useState(true);

  const fetchMovies = () => fetch(API).then(r => r.json()).then(setMovies);
  const fetchActors = () => fetch(`${API}/actors`).then(r => r.json()).then(setActors);

  useEffect(() => {
    Promise.all([fetchMovies(), fetchActors()]).finally(() => setLoading(false));
  }, []);

  const handleSaveMovie = (movieData) => {
    if (editingMovie) {
      fetch(`${API}/${editingMovie.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(movieData)
      }).then(() => { setEditingMovie(null); fetchMovies(); });
    } else {
      fetch(API, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(movieData)
      }).then(() => fetchMovies());
    }
  };

  const handleDeleteMovie = (id) => {
    if (window.confirm('Сигурни ли сте?')) {
      fetch(`${API}/${id}`, { method: 'DELETE' }).then(() => fetchMovies());
    }
  };

  return (
    <div style={{ maxWidth: '800px', margin: '40px auto', padding: '0 20px', fontFamily: 'Arial, sans-serif' }}>
      <h1>🎬 Каталог Филми и Актьори (CRUD)</h1>

      <MovieForm
        onSubmit={handleSaveMovie}
        editingMovie={editingMovie}
        onCancel={() => setEditingMovie(null)}
        availableActors={actors}
      />

      {loading ? <p>Зареждане...</p> : movies.map(movie => (
        <MovieCard key={movie.id} movie={movie}
                   onDelete={handleDeleteMovie}
                   onEdit={m => setEditingMovie(m)} />
      ))}
    </div>
  );
}
