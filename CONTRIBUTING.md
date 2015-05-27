# Conventions

## Branching Strategy

We are using a strategy very similar to [praqma's gitflow](http://www.praqma.com/resources/papers/git-flow).
The way it mainly differs from [@nvie's model](http://nvie.com/posts/a-successful-git-branching-model/) is that we don't have two long living branches i.e. master and develop. is only master, and only product ready code gets merged into master after going through a automated testing, code reviews and manual testing. 

### Branch Naming

* **Bug Fixes and New Features**: All bug fixes and new features are handled in their own branches. Once the changes are ready, a pull request is created for them to be merged with the master repository
* **Releases**: Once we have a release candidate in the development branch, a release branch is created and changes are merged from the development branch. Once the release is tested, changes are merged to master and the release branch is deleted

Here is the branch naming convention we are using:

```
<BRANCH_NAME>   ::= <task_category>/<issue_number>/<description> [/main]

<task_category> ::= bug|feat|wip|task|exp
<issue_number>  ::= <github_issue_number>
<description>   ::= <description_of_the_task_separated_by_underscores>
```

where the tas categories are:

- **bug**  : Bug fixes
- **feat** : Enhancements / New features
- **wip**  : Work in progress
- **task** : Tasks that are not bugs, enhancements or recurring
- **exp**  : Experiments 

For Example:
- wip/8/contributing/main
- wip/28/docker/main
- feat/18/order_documents
- bug/16/restrict_delete_default_directory

# Contributing

All changes need to be reviewed before they get merged into the master
branch. We rely on github pull requests to suggest changes to the code
base.

## Setup (One time)

1. Fork the project. (For details about forking, visit: https://help.github.com/articles/fork-a-repo/)

2. Add a remote for the upstream repository
```
git remote add upstream git@github.com:Penneo/Symfony2.git
```

*Make sure to configure user for the repository*
```
git config user.name  |FULL_NAME|
git config user.email |OFFICIAL_EMAIL|
```


## Creating Pull Requests

1. Create a github issue, if it doesn't exist already

2. Create branch for the change to be implemented
	```
	git checkout -b bug/1/name upstream/master
	
	# or in two steps:
	
	git checkout master      # Make sure that master is synced
	git checkout -b bug/1/name
	```

3. Implement changes on the branch

4. Sync with the master
	```
	$ git fetch upstream
	```

5. Make sure that the PR can be easily merged. For reference, have a look [here](https://github.com/edx/edx-platform/wiki/How-to-Rebase-a-Pull-Request)

	```
	git rebase -i upstream/master
	```

6. Create a pull request from your branch to upstream/master

## Accepting Pull Requests

(e.g. branch 'feature_x' from some user 'bob')

Add 'bob' and 'penneo' as a remote:

    git remote add penneo -t master -f git@github.com:penneo/REPONAME.git            # Add a remote for the truth repo
	git remote add bob -t feature_x -f git@github.com:bob/REPONAME.git               # Add a remote for the user

Now, rebase feature branch onto the target branch:

	git checkout -b feature_x bob/feature_x
	git rebase penneo/master

Add a local branch based on the branch that you are merging into:

    git checkout -b merge/feature_x penneo/master

Merge the feature in:

	git merge --no-ff feature_x                        # Put "Fixes #issue_id, PR Close #pull_request_id" in the commit message

Run the tests:

	phpunit -c app/

If everything looks good, push the changes and remove the local feature branch:

	git push penneo HEAD:development
	git branch -D feature_x
	git branch -D merge_feature_x
	git remote rm bob
	git remote rm penneo

### Check the graph

Check that the commits are merged into the long lived branch and we
have a linear history.

	git log --graph -n 3
