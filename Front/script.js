const API_URL = 'https://localhost:7195/api/Post';
const postForm = document.getElementById('post-form');
const postsList = document.getElementById('posts-list');

    // Fetch all posts
    async function fetchPosts() {
        try {
            const response = await fetch(API_URL);
            if (!response.ok) throw new Error('Failed to fetch posts');
            const posts = await response.json();
            displayPosts(posts);
        } 
        catch (error) {
            console.error('Error: ', error);
        }
    }

    // Display posts
    function displayPosts(posts) {
        postsList.innerHTML = '';
        posts.forEach(post => {
            const postElement = document.createElement('div');
            postElement.className = 'post';
            postElement.innerHTML = `
                    <h3>${post.title}</h3>
                    <p>${post.content}</p>
                    <p>Author: ${post.authorName}</p>
                    <p>Community: ${post.communityName}</p>
                    <button onclick="editPost(${post.id})">Edit</button>
                    <button onclick="deletePost(${post.id})">Delete</button>
                `;
            postsList.appendChild(postElement);
        });
    }

    // Handle form submission (create/update post)
    postForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const postId = document.getElementById('post-id').value;
        const post = {
            title: document.getElementById('title').value,
            content: document.getElementById('content').value,
            authorName: document.getElementById('author-name').value,
            communityName: document.getElementById('community-name').value
        };

        try {
            const url = API_URL;
            const method = 'POST';
            const response = await fetch(url, {
                method: method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(post)
            });
            if (!response.ok) throw new Error('Failed to submit post');
            postForm.reset();
            document.getElementById('post-id').value = '';
            fetchPosts();
        } catch (error) {
            console.error('Error:', error);
        }
    });

    // Edit post
    async function editPost(id) {
        try {
            const response = await fetch(`${API_URL}/${id}`);
            if (!response.ok) throw new Error('Failed to fetch post');
            const post = await response.json();
            document.getElementById('post-id').value = post.id;
            document.getElementById('title').value = post.title;
            document.getElementById('content').value = post.content;
            document.getElementById('author-name').value = post.authorName;
            document.getElementById('community-name').value = post.communityName;
        } catch (error) {
            console.error('Error:', error);
        }
    }

    // Delete post
    async function deletePost(id) {
        if (confirm('Are you sure you want to delete this post?')) {
            try {
                const response = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
                if (!response.ok) throw new Error('Failed to delete post');
                fetchPosts();
            } catch (error) {
                console.error('Error:', error);
            }
        }
    }

    // Initial fetch of posts
    fetchPosts();