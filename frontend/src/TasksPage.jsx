import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './TasksPage.css';

const TasksPage = () => {
  const [tasks, setTasks] = useState([]);
  const [showForm, setShowForm] = useState(false);
  const [isEditing, setIsEditing] = useState(false); // Track if we're editing
  const [currentTask, setCurrentTask] = useState(null)
  const [newTask, setNewTask] = useState({
    title: '',
    description: '',
    dueDate: '',
    isCompleted: false,
    createdAt: new Date().toISOString(),
  });

  useEffect(() => {
    fetchTasks();
  }, []);

  const fetchTasks = async () => {
    try { 
      console.log(localStorage.getItem('token','abcc'));

      const response = await axios.get('http://localhost:5099/api/Tasks', {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      });
      setTasks(response.data);
    } catch (error) {
      console.error('Error fetching tasks:', error);
    }
  };

  const handleAddTask = async (e) => {
    e.preventDefault();
    const userId = parseInt(localStorage.getItem('userId'));
    console.log("Retrieved User ID:", userId);
    if (!userId) {
      console.error("User ID is not available. Make sure the user is logged in.");
      return; // Exit the function if userId is not available
  }
    const taskToAdd = { ...newTask, userId: parseInt( localStorage.getItem('userId')) };

    try {
      const response = await axios.post('http://localhost:5099/api/Tasks', taskToAdd, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      });
      setTasks([...tasks, response.data]);
      setNewTask({ title: '', description: '', dueDate: '', isCompleted: false, createdAt: new Date().toISOString() });
      setShowForm(false); // Hide the form after adding the task
    } catch (error) {
      console.error('Error adding task:', error);
    }
  };

  const handleDeleteTask = async (taskId) => {
    try {
      await axios.delete(`http://localhost:5099/api/Tasks/${taskId}`, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      });
      setTasks(tasks.filter((task) => task.taskId !== taskId));
    } catch (error) {
      console.error('Error deleting task:', error);
    }
  };

  /*const handleEditTask = (taskId) => {
    console.log('Edit task with ID:', taskId);
  };

  const toggleForm = () => {
    setShowForm(!showForm);
  };*/
  const handleEditTask = async (taskId, updatedTask) => {
    try {
      // Send PUT request to update the task
      await axios.put(`http://localhost:5099/api/Tasks/${taskId}`, updatedTask, {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      });

      // Update the task locally in the state
      setTasks((prevTasks) =>
        prevTasks.map((task) =>
          task.taskId === taskId ? { ...task, ...updatedTask } : task
        )
      );
      setIsEditing(false);
      setNewTask({ title: '', description: '', dueDate: '', isCompleted: false });
      setShowForm(false);
    
      // After editing, hide the form
    } catch (error) {
      console.error('Error updating task:', error);
    }
  };

  const toggleForm = () => {
    setShowForm(!showForm);
    if (isEditing) {
      setIsEditing(false); // If we are editing, close the form and reset
    }
  };

  const handleEditClick = (task) => {
    setCurrentTask(task);
    setNewTask({
      title: task.title,
      description: task.description,
      dueDate: task.dueDate,
      isCompleted: task.isCompleted,
    });
    setIsEditing(true);
    setShowForm(true); // Show the form when editing
  };

  const handleFormSubmit = async (e) => {
    e.preventDefault();
    if (isEditing && currentTask) {
      await handleEditTask(currentTask.taskId, newTask); // Call edit handler if we're editing
    } else {
      await handleAddTask(e); // Otherwise, add a new task
    }
  };
  return (
    <div className="tasks-page">
      <h1>Your To-Do List</h1>

      {/* Add New Task Card */}
      <div className="add-task-card">
        <div className="card-header">
          <h2>{isEditing ? 'Edit Task' : 'Add New Task'}</h2>
          <button className="add-task-toggle" onClick={toggleForm}>
            {showForm ? '−' : '+'}
          </button>
        </div>

        {showForm && (
          <form className="task-form" onSubmit={handleFormSubmit}>
            <input
              type="text"
              placeholder="Title"
              value={newTask.title}
              onChange={(e) => setNewTask({ ...newTask, title: e.target.value })}
              required
            />
            <textarea
              placeholder="Description"
              value={newTask.description}
              onChange={(e) => setNewTask({ ...newTask, description: e.target.value })}
            />
            <input
              type="date"
              value={newTask.dueDate}
              onChange={(e) => setNewTask({ ...newTask, dueDate: e.target.value })}
            />
            <label>
              <input
                type="checkbox"
                checked={newTask.isCompleted}
                onChange={(e) => setNewTask({ ...newTask, isCompleted: e.target.checked })}
              />
              Completed
            </label>
            <button type="submit" className="add-task-button">
              {isEditing ? 'Save Changes' : 'Add Task'}
            </button>
          </form>
        )}
      </div>

      {/* Task List */}
      <div className="task-list">
        {tasks.map((task) => (
          <div key={task.taskId} className="task-card">
            <h3>{task.title}</h3>
            <p>{task.description}</p>
            <p>Due: {task.dueDate}</p>
            <p>Completed: {task.isCompleted ? 'Yes' : 'No'}</p>
            <div className="task-actions">
              <button onClick={() => handleEditClick(task)} className="edit-button">Edit</button>
              <button onClick={() => handleDeleteTask(task.taskId)} className="delete-button">Delete</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default TasksPage;