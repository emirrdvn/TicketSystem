import axios from './axios';

export const categoryAPI = {
  // Get all categories
  getAllCategories: async () => {
    const response = await axios.get('/category');
    return response.data;
  },

  // Get category by ID
  getCategoryById: async (id) => {
    const response = await axios.get(`/category/${id}`);
    return response.data;
  },

  // Create category
  createCategory: async (categoryData) => {
    const response = await axios.post('/category', categoryData);
    return response.data;
  },

  // Update category
  updateCategory: async (id, categoryData) => {
    const response = await axios.put(`/category/${id}`, categoryData);
    return response.data;
  },

  // Delete category
  deleteCategory: async (id) => {
    await axios.delete(`/category/${id}`);
  }
};
