CMAKE_MINIMUM_REQUIRED(VERSION 3.7.2)
PROJECT(MultiTargetBuilder)
# Only UNIX systems.
SET(VERSION_MAJOR 0)
SET(VERSION_MINOR 1)
SET(VERSION_REVISION 6)
SET(VERSION_STATE rc)
SET(VERSION ${VERSION_MAJOR}.${VERSION_MINOR}${VERSION_STATE}${VERSION_REVISION} )
IF( UNIX )
	Find_PACKAGE(UnixCommands REQUIRED)

	# Create distribution tarball.
	SET( TARGETDIR "${PROJECT_NAME}-${VERSION}")
	ADD_CUSTOM_TARGET(	distribution
			COMMENT "Creating distrubtion file."
			COMMAND mkdir -p ${TARGETDIR}
			COMMAND ${CP} -r  ${CMAKE_CURRENT_SOURCE_DIR}/Editor
			${CMAKE_CURRENT_SOURCE_DIR}/build.sh
			${CMAKE_CURRENT_SOURCE_DIR}/package.json
			${CMAKE_CURRENT_SOURCE_DIR}/README.md
			${CMAKE_CURRENT_SOURCE_DIR}/CHANGELOG.md
			${CMAKE_CURRENT_SOURCE_DIR}/Tests
			${TARGETDIR}
			COMMAND ${TAR} cf - ${TARGETDIR} | ${GZIP} -c > ${TARGETDIR}.tar.gz
			COMMAND ${RM} -r ${TARGETDIR} )

	# Create source distribution tarball.
	SET( TARGETSOURCEDIR "${PROJECT_NAME}-Source-${VERSION}")
	ADD_CUSTOM_TARGET(	distribution-source
			COMMENT "Creating distrubtion file."
			COMMAND mkdir -p ${TARGETSOURCEDIR}
			COMMAND ${CP} -r   ${CMAKE_CURRENT_SOURCE_DIR}/Editor
			${CMAKE_CURRENT_SOURCE_DIR}/build.sh
			${CMAKE_CURRENT_SOURCE_DIR}/package.json
			${CMAKE_CURRENT_SOURCE_DIR}/README.md
			${CMAKE_CURRENT_SOURCE_DIR}/CHANGELOG.md
			${CMAKE_CURRENT_SOURCE_DIR}/Tests
			${CMAKE_CURRENT_SOURCE_DIR}/LICENSE
			${TARGETSOURCEDIR}
			COMMAND ${TAR} cf - ${TARGETSOURCEDIR} | ${GZIP} -c > ${TARGETSOURCEDIR}.tar.gz
			COMMAND ${RM} -r ${TARGETSOURCEDIR} )

ENDIF()